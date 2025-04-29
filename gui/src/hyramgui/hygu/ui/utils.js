/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */

// utils.js
.pragma library


/**
 * Load with:
 * import "../hygu/ui/utils.js" as Utils
 */

function testFunc()
{
  console.log("test utils");
}


function isNullish(val)
{
  return (val === null || val === undefined || val === "");
}


function clearImage(qimg)
{
  qimg.source = "";
  qimg.filename = "";
}


function updateImage(qimg, fl)
{
  let val = fl ? fl : "";
  if (val === "" || val === null)
  {
    qimg.visible = false;
    return;
  }
  qimg.visible = true;
  qimg.source = 'file:' + val;
  qimg.filename = val;
}


function showChoiceParam(textElem, param)
{
  textElem.text = "<strong>" + param.label + "</strong>: " + param.value_display;
}


function showBasicParam(textElem, param)
{
  textElem.text = "<strong>" + param.label + "</strong>: " + param.value;
}


/**
 * Display parameter value with label
 * @param elem
 * @param param
 */
function showParam(elem, param, nDecimals)
{
  let val = param.value;
  let fmtVal = val;
  if (isNullish(nDecimals))
  {
    nDecimals = val > 1 ? 3 : 6;
  }
  var mult = 10**nDecimals;
  fmtVal = Math.round(val * mult) / mult;

  elem.text = "<strong>" + param.label_rtf + "</strong>: " + fmtVal + " " + param.get_unit_disp;
}


function showStrValue(elem, label, strval, unit)
{
  elem.text = "<strong>" + label + "</strong>: " + strval + " " + unit;
}


function randomScalingFactor() {
  return Math.random().toFixed(1);
}


/*
 * Get unicode superscript version of number, period, or comma. (Chartjs canvas can't do superscript)
 */
function getUnicodeSuperscript(chr) {
  if (chr === '0') return '\u2070';
  if (chr === '1') return '\u00B9';
  if (chr === '2') return '\u00B2';
  if (chr === '3') return '\u00B3';
  if (chr === '4') return '\u2074';
  if (chr === '5') return '\u2075';
  if (chr === '6') return '\u2076';
  if (chr === '7') return '\u2077';
  if (chr === '8') return '\u2078';
  if (chr === '9') return '\u2079';
  if (chr === '.') return '\u22C5';
  if (chr === ',') return '\u1D112';
  if (chr === '-') return '\u207B';
  else return '';
}


/**
 * Converts input to unicode superscript.
 * @param num
 * @returns {string}
 */
function numToSuperscript(num) {
  var numStr = num.toString();
  var result = '';
  for (let i = 0; i < numStr.length; i++) {
    result += getUnicodeSuperscript(numStr.charAt(i));
  }
  return result;
}

/**
 * Converts number to scientific notation. Assumes logarithmic values (10, 1000, etc.)
 * e.g.: 1000 -> 10^3 with exponent represented by unicode superscript
 */
function convertValueToSuperscriptString(val) {
  let nZeros = Math.floor(Math.log10(val));
  let valStr = "10";
  if (nZeros < 0) {
    valStr += numToSuperscript("-") + numToSuperscript(Math.abs(nZeros));
  } else {
    valStr += numToSuperscript(nZeros);
  }
  return valStr;
}


function getLogTicks (chartObj) {
  // Replace ticks with pretty log values, up to max.
  const minVal = chartObj.ticks[0];
  const maxVal = chartObj.ticks.pop();
  const ticks = [
    1e-5, 1e-4, 1e-3, 1e-2, 1e-1, 1e0,
    1e1, 1e2, 1e3, 1e4, 1e5,
    1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12,
  ];
  chartObj.ticks.splice(0, chartObj.ticks.length);

  for (let i=0; i < ticks.length; i++) {
    let val = ticks[i];
    if (val > maxVal) break;

    if (val >= minVal) {
      chartObj.ticks.push(val);
    }
  }
}


function getLogYAxisTicks (chartObj) {
  // Replace ticks with pretty log values, up to max. Inverted because y-axis log ticks stored largest to smallest.
  const maxVal = chartObj.ticks[0];
  const minVal = chartObj.ticks.pop();
  const ticks = [
    1e-10, 1e-9, 1e-8, 1e-7, 1e-6,  // don't go smaller because will get into fp/rounding errors
    1e-5, 1e-4, 1e-3, 1e-2, 1e-1, 1e0,
    1e1, 1e2, 1e3, 1e4, 1e5,
    1e6, 1e7, 1e8, 1e9, 1e10
  ];
  chartObj.ticks.splice(0, chartObj.ticks.length);

  for (let i = ticks.length-1; i >= 0; i--)
  {
    let val = ticks[i];
    if (val < minVal) break;

    if (val <= maxVal)
    {
      chartObj.ticks.push(val);
    }
  }
}
