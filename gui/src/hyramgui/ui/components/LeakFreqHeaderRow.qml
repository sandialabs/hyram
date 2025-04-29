/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */
import QtQuick 2.12
import QtQuick.Layouts


RowLayout {
    Text {text: "Leak Size"; Layout.preferredWidth: 180; font.bold: true; horizontalAlignment: Text.AlignCenter; }
    Text {text: "Mu"; Layout.preferredWidth: 100; font.bold: true; horizontalAlignment: Text.AlignCenter; }
    Text {text: "Sigma"; Layout.preferredWidth: 100; font.bold: true; horizontalAlignment: Text.AlignCenter; }
    Text {text: "Mean"; Layout.preferredWidth: 100; font.bold: true; horizontalAlignment: Text.AlignCenter; }
    Text {text: "5th"; Layout.preferredWidth: 100; font.bold: true; horizontalAlignment: Text.AlignCenter; }
    Text {text: "Median"; Layout.preferredWidth: 100; font.bold: true; horizontalAlignment: Text.AlignCenter; }
    Text {text: "95th"; Layout.preferredWidth: 100; font.bold: true; horizontalAlignment: Text.AlignCenter; }
}
