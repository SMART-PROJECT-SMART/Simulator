namespace Simulation.Common.Enums
{
    public static class TelemetryCategories
    {
        public const string Flight = "Flight";
        public const string Armed = "Armed";
        public const string Surveillance = "Surveillance";
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TelemetryCategoryAttribute : Attribute
    {
        public string Category { get; }

        public TelemetryCategoryAttribute(string category) => Category = category;
    }

    public enum TelemetryFields
    {
        [TelemetryCategory(TelemetryCategories.Flight)]
        DragCoefficient,

        [TelemetryCategory(TelemetryCategories.Flight)]
        LiftCoefficient,

        [TelemetryCategory(TelemetryCategories.Flight)]
        ThrottlePercent,

        [TelemetryCategory(TelemetryCategories.Flight)]
        CruiseAltitude,

        [TelemetryCategory(TelemetryCategories.Flight)]
        Latitude,

        [TelemetryCategory(TelemetryCategories.Flight)]
        Longitude,

        [TelemetryCategory(TelemetryCategories.Flight)]
        Altitude,

        [TelemetryCategory(TelemetryCategories.Flight)]
        CurrentSpeedKmph,

        [TelemetryCategory(TelemetryCategories.Flight)]
        YawDeg,

        [TelemetryCategory(TelemetryCategories.Flight)]
        PitchDeg,

        [TelemetryCategory(TelemetryCategories.Flight)]
        RollDeg,

        [TelemetryCategory(TelemetryCategories.Flight)]
        ThrustAfterInfluence,

        [TelemetryCategory(TelemetryCategories.Flight)]
        FuelAmount,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        DataStorageUsedGB,
    }
}
