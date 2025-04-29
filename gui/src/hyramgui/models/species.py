"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""

class Species:
    id: int
    name: str
    key: str
    choked_flow: float
    p_crit: float  # [MPa]
    lfl: float

    def __init__(self, id, name, key, choked_flow, p_crit, lfl=0.):
        self.id = id
        self.name = name
        self.key = key
        self.choked_flow = float(choked_flow)
        self.p_crit = float(p_crit)
        self.lfl = float(lfl)


hydrogen = Species(0, "Hydrogen", "h2", 1.899, 1.296400, 0.04)
methane = Species(1, "Methane", "ch4", 1.839, 4.599200, 0.05)
propane = Species(2, "Propane", "c3h8", 1.719, 4.251200, 0.021)
nitrogen = Species(3, "Nitrogen", "n2", 1.889, 0)
co2 = Species(4, "Carbon Dioxide", "co2", 1.829, 0)
ethane = Species(5, "Ethane", "c2h6", 1.769, 0, 0.03)
nbutane = Species(6, "Butane", "n-c4h10", 1.689, 0, 0.018)
isobutane = Species(7, "Isobutane", "hc(ch3)3", 1.689, 0, 0.018)
npentane = Species(8, "Pentane", "n-c5h12", 0, 0, 0.014)
isopentane = Species(9, "Isopentane", "ch(ch3)2(c2h5)", 0, 0, 0.014)
nhexane = Species(10, "Hexane", "n-c6h14", 0, 0, 0.012)
blend = Species(99, "Blend", "blend", -1.0, -1.0, 0)

species_dict = {
    hydrogen.key: hydrogen,
    methane.key: methane,
    propane.key: propane,
    nitrogen.key: nitrogen,
    co2.key: co2,
    ethane.key: ethane,
    nbutane.key: nbutane,
    isobutane.key: isobutane,
    npentane.key: npentane,
    isopentane.key: isopentane,
    nhexane.key: nhexane,
    blend.key: blend,
}