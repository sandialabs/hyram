"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import logging
import threading
import time
from concurrent.futures import ProcessPoolExecutor as Pool
from datetime import datetime
from multiprocessing import Manager

# TODO: clean this
try:
    import app_settings
except ImportError or ModuleNotFoundError:
    try:
        from .. import app_settings
    except ImportError or ModuleNotFoundError:
        from ... import app_settings

from ..models import models


log = logging.getLogger(app_settings.APPNAME)


class AnalysisThread(threading.Thread):
    """
    Long-running thread manages analysis multiprocessing pool.

    Attributes
    ----------
    pool : ProcessPoolExecutor
        Pool variant which allows children to initialize new sub-processes.

    state : models.ModelBase
        Model representing current form parameters.

    is_shutdown : bool
        Flag indicating whether pool has finished shutting down.

    manager : Manager
        Enables access to shared status_dict of analysis statuses (i.e. for canceling)

    Notes
    -----
    New analyses are activated on a polling basis, wherein Submit Button activates new_analysis flag.
    Thread.run polls for this flag and initiates new analysis when set.

    """
    pool: Pool
    state: type(models.ModelBase)
    # number of analyses executing
    _num_active: int = 0
    _num_complete: int = 0
    # max number of analyses allowed in queue at a time
    _max_active: int = 4
    # increasing identifier for analyses; must be unique.
    _current_id: int = 0
    _do_shutdown: bool = False
    is_shutdown = False
    manager: Manager

    # list of analyses waiting to execute. Each entry is tuple: (id, state_copy, analysis_func, started_callback, finished_callback)
    _queue: list = []
    # map of data for analyses currently executing {id: callback function}
    _active_analysis_map: dict = {}
    _state_map: dict = {}
    # track futures in case of error
    _future_map: dict = {}
    status_dict: dict

    def __init__(self, state: type(models.ModelBase), num_processes=None):
        """Initializes pool with specified process count.

        Parameters
        ----------
        state : type(models.ModelBase)
            model object representing parameters in form.
        num_processes : int or None
            Sets number of processes. If none, pool will be created with # of processes matching system processor count.

        Notes
        -----
        pool is stored in settings so backend can access it as needed.

        """
        self._log("Initializing AnalysisThread with pool.")
        super().__init__()

        self.manager = Manager()
        self.status_dict = self.manager.dict()

        if num_processes is None:
            self.pool = Pool()
        else:
            self.pool = Pool(max_workers=num_processes)

        self.state = state
        app_settings.MP_POOL = self.pool
        self._log("AnalysisThread initialized.")

    def run(self):
        """
        Polls for new analysis requests and executes analyses via processing pool.

        Notes
        -----
        Analysis state objects are QObjects which CANNOT be pickled so data management is done with dicts.
        Called upon thread creation. Ends thread when it returns.

        """
        self._log("Begin thread polling.")

        while True:
            if self._queue and not self._do_shutdown:  # process one at a time

                if self._num_active >= self._max_active:
                    # pool is full
                    time.sleep(1.0)
                    continue

                (analysis_id, analysis_func, state_copy, started_callback, finished_callback) = self._queue.pop(0)

                # save copy of analysis state object for later use as results display model
                self._num_active += 1
                state_copy.set_id(analysis_id)
                state_copy.started_at = datetime.now()
                self._state_map[analysis_id] = state_copy

                params = state_copy.get_prepped_param_dict()
                if app_settings.USE_SUBPROCESS:
                    self.status_dict[analysis_id] = True
                    future = self.pool.submit(analysis_func, analysis_id, params, self.status_dict)
                    # map to easily retrieve id, even if critical error
                    self._future_map[future] = analysis_id
                    future.add_done_callback(self._process_results)

                else:
                    log.info("Pool disabled. Attempting analysis in same process.")
                    try:
                        res = analysis_func(analysis_id, params, self.status_dict)
                        self._process_results(future=None, results=res)
                    except Exception as err:
                        log.exception("Exception occurred during non-pool analysis call")
                        self._dev_process_exception(err=err)

                self._active_analysis_map[analysis_id] = finished_callback

                started_callback(analysis_id)

                time.sleep(1.0)

            if self._do_shutdown:
                self._log("shutdown flag set - halting pool and workers")
                if self.pool is not None:
                    # wait=True to ensure child processes aren't orphaned
                    self.pool.shutdown(wait=True)
                    self._log("Pool shutdown complete")
                self.is_shutdown = True
                break

            time.sleep(1.0)

        self._log("Halting thread.")
        return

    def request_new_analysis(self, state_obj, analysis_func, started_callback: callable, finished_callback: callable) -> int or None:
        """Begins flow for new analysis by obtaining id and adding it to queue with callbacks.

        Parameters
        ----------
        state_obj : type(models.ModelBase)
            Deep copy of state model backing the analysis.
        analysis_func : function
            Reference to analysis function to call
        started_callback : func
            Function to call once analysis processing begins.
        finished_callback : func
            Function to call once analysis processing finishes.

        Returns
        -------
        int
            Unique analysis id.

        """
        self._log(f'new analysis requested ({self._num_active} / {self._max_active} active)')
        analysis_id = self._get_next_id()
        # state must not have any listeners from parent objects, or they'll get cloned and must be pickleable
        self._queue.append((analysis_id, analysis_func, state_obj, started_callback, finished_callback))
        return analysis_id

    def cancel_analysis(self, a_id):
        """Sets cancellation flag for in-progress analysis. Backend -may- use to halt analysis early. """
        if a_id in self.status_dict:
            self.status_dict[a_id] = False

    def shutdown(self):
        """Set shutdown flag to tell thread it's time to shut down. """
        for a_id in self.status_dict.keys():  # cancel all in-progress
            self.status_dict[a_id] = False
        self._do_shutdown = True

    def _process_results(self, future, results=None):
        """Updates pending state object with results of analysis.

        Parameters
        ----------
        future : Future or None
            Result function which returns dict of analysis results. Will only be None if multiprocessing off.
        results : dict
            Analysis results passed directly when multiprocessing disabled.

        """
        analysis_id = self._future_map[future] if future is not None else results['analysis_id']

        self._log(f"processing results for analysis {analysis_id}")
        self._num_active -= 1
        self._num_complete += 1

        try:
            if results is None:
                results = future.result(timeout=None)

        except Exception as err:
            # Unhandled exception during analysis so manually set result data for GUI.
            msg = "Critical error encountered during analysis call - check log for details."
            results = {
                'status': -1,
                'error': err,
                'message': msg
            }
            self._log(msg)
            log.exception(err)

        # Update status of cloned state object
        state = self._state_map[analysis_id]
        state.is_finished = True
        state.finished_at = datetime.now()

        if 'error' in results:
            # self._log("Error encountered during analysis.")
            log.error(results['error'])

        else:
            log.info("Analysis complete - processing data from sub-process.")

        analysis_finished_callback = self._active_analysis_map.pop(analysis_id, None)
        if analysis_finished_callback is None:
            raise KeyError("Analysis ID not found when processing results")

        analysis_finished_callback(state, results)

    def _dev_process_exception(self, err):
        """Handles exception when pooling is not active. """
        self._num_active -= 1
        self._log(f"Analysis task failed with error: {err}")

    def _get_next_id(self):
        """Returns unique int id for next submitted analysis. """
        self._current_id += 1
        return self._current_id

    def get_curr_id(self):
        return self._current_id

    def _log(self, msg: str):
        log.info(f"Thread - {msg}")
