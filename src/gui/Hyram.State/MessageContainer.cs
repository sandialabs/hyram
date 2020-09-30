namespace SandiaNationalLaboratories.Hyram
{

    public static class MessageContainer
    {
        public const string LiquidReleasePressureInvalid =
            "Saturated release pressure must be between ambient pressure and critical pressure (1.29 MPa)";

        public const string FuelFlowChoked = "Fuel flow is choked; release pressure is less than critical ratio * ambient pressure";
    }
}
