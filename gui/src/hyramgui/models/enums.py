"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
from hyramgui.hygu.utils.distributions import BaseChoiceList


class FuelMixType(BaseChoiceList):
    """Fuel mixture options """
    keys = ['h2', 'ch4', 'c3h8', 'nist1', 'nist2', 'rg2', 'gu1', 'gu2', 'man']
    labels = ['Hydrogen', 'Methane', 'Propane', 'NIST1', 'NIST2', 'RG2', 'GU1', 'GUI2', 'Blend (manual)']
    h2 = 'h2'
    ch4 = 'ch4'
    c3h8 = 'c3h8'
    nist1 = 'nist1'
    nist2 = 'nist2'
    rg2 = 'rg2'
    gu1 = 'gu1'
    gu2 = 'gu2'
    man = 'man'


class PhaseType(BaseChoiceList):
    keys = ['fluid', 'gas', 'liquid']  # fluid option will be converted to None in backend
    labels = ['Fluid', 'Saturated vapor', 'Saturated liquid']
    fluid = 'fluid'
    svap = 'gas'
    sliq = 'liquid'


class NozzleType(BaseChoiceList):
    keys = ['birc', 'bir2', 'ewan', 'molk', 'yuce']
    labels = ['Birch1', 'Birch2', 'Ewan/Moodie', 'Molkov', 'Yuceil/Otugen']
    bir1 = 'birc'
    bir2 = 'bir2'
    ewa = 'ewan'
    mol = 'molk'
    yuc = 'yuce'


class OverpressureType(BaseChoiceList):
    keys = ['bst', 'tnt', 'bauwens']
    labels = ['BST', 'TNT', 'Bauwens']
    bst = 'bst'
    tnt = 'tnt'
    bau = 'bauwens'


class MachSpeed(BaseChoiceList):
    keys = ['s0.2', 's0.35', 's0.7', 's1', 's1.4', 's2', 's3', 's4', 's5.2']
    labels = ['0.2', '0.35', '0.7', '1.0', '1.4', '2.0', '3', '4', '5.2']
    s0d2 = 's0.2'
    s0d35 = 's0.35'
    s0d7 = 's0.7'
    s1 = 's1'
    s1d4 = 's1.4'
    s2 = 's2'
    s3 = 's3'
    s4 = 's4'
    s5d2 = 's5.2'


class ThermalProbitModel(BaseChoiceList):
    keys = ['eise', 'tsao', 'tno', 'lees']
    labels = ['Eisenberg', 'Tsao', 'TNO', 'lees']
    eis = 'eise'
    tsa = 'tsao'
    tno = 'tno'
    lee = 'lees'


class OverpressureProbitModel(BaseChoiceList):
    keys = ['leis', 'lhse', 'head', 'coll']
    labels = ['Lung (Eisenberg)', 'Lung (HSE)', 'Head impact', 'Collapse']
    eis = 'leis'
    hse = 'lhse'
    hea = 'head'
    col = 'coll'


class LeakSizeType(BaseChoiceList):
    keys = ['ld01', 'ld1', 'l1', 'l10', 'l100']
    labels = ['0.01% leak size', '0.10% leak size', '1% leak size', '10% leak size', '100% leak size']
    ld01 = 'ld01'
    ld1 = 'ld1'
    l1 = 'l1'
    l10 = 'l10'
    l100 = 'l100'


class FailureDistribution(BaseChoiceList):
    keys = ['beta', 'ev', 'ln']
    labels = ['Beta', 'Expected Value', 'LogNormal']
    beta = 'beta'
    ev = 'ev'
    ln = 'ln'
