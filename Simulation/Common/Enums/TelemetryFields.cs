using System.Reflection;

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

    public enum FlightTelemetryFields
    {
        Mass,
        FrontalSurface,
        WingsSurface,
        DragCoefficient,
        LiftCoefficient,
        ThrustMax,
        MaxCruiseSpeedKmph,
        MaxAccelerationMps2,
        MaxDecelerationMps2,
        CruiseAltitude,
        Latitude,
        Longitude,
        Altitude,
        CurrentSpeedKmph,
        Horizontal_Acceleration,
        Vertical_Acceleration,
        YawDeg,
        PitchDeg,
        RollDeg,
        AngleBetweenPlaneAndGround,
        ThrustAfterInfluence,
    }

    public enum ArmedTelemetryFields
    {
        WeaponSystemStatus,
        IsWeaponSystemArmed,
        HellfireAmmo,
        GriffinAmmo,
        SpikeNLOSAmmo,
        JDAMAmmo,
        WeaponSystemHealth,
    }

    public enum SurveillanceTelemetryFields
    {
        SensorSystemStatus,
        DataStorageUsedGB,
        DataStorageCapacityGB,
        ElectroOpticalSensorStatus,
        InfraredImagingSensorStatus,
        SyntheticApertureRadarStatus,
        SIGINTSensorStatus,
        ELINTSensorStatus,
        WeatherRadarStatus,
        LaserDesignatorStatus,
        MultiSpectralImagingStatus,
        HyperspectralImagingStatus,
        CommunicationsRelayStatus,
    }

    public enum TelemetryFields
    {
        [TelemetryCategory(TelemetryCategories.Flight)]
        Mass,

        [TelemetryCategory(TelemetryCategories.Flight)]
        FrontalSurface,

        [TelemetryCategory(TelemetryCategories.Flight)]
        WingsSurface,

        [TelemetryCategory(TelemetryCategories.Flight)]
        DragCoefficient,

        [TelemetryCategory(TelemetryCategories.Flight)]
        LiftCoefficient,

        [TelemetryCategory(TelemetryCategories.Flight)]
        ThrustMax,

        [TelemetryCategory(TelemetryCategories.Flight)]
        MaxCruiseSpeedKmph,

        [TelemetryCategory(TelemetryCategories.Flight)]
        MaxAccelerationMps2,

        [TelemetryCategory(TelemetryCategories.Flight)]
        MaxDecelerationMps2,

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
        Horizontal_Acceleration,

        [TelemetryCategory(TelemetryCategories.Flight)]
        Vertical_Acceleration,

        [TelemetryCategory(TelemetryCategories.Flight)]
        YawDeg,

        [TelemetryCategory(TelemetryCategories.Flight)]
        PitchDeg,

        [TelemetryCategory(TelemetryCategories.Flight)]
        RollDeg,

        [TelemetryCategory(TelemetryCategories.Flight)]
        AngleBetweenPlaneAndGround,

        [TelemetryCategory(TelemetryCategories.Flight)]
        ThrustAfterInfluence,

        [TelemetryCategory(TelemetryCategories.Armed)]
        WeaponSystemStatus,

        [TelemetryCategory(TelemetryCategories.Armed)]
        IsWeaponSystemArmed,

        [TelemetryCategory(TelemetryCategories.Armed)]
        HellfireAmmo,

        [TelemetryCategory(TelemetryCategories.Armed)]
        GriffinAmmo,

        [TelemetryCategory(TelemetryCategories.Armed)]
        SpikeNLOSAmmo,

        [TelemetryCategory(TelemetryCategories.Armed)]
        JDAMAmmo,

        [TelemetryCategory(TelemetryCategories.Armed)]
        WeaponSystemHealth,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        SensorSystemStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        DataStorageUsedGB,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        DataStorageCapacityGB,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        ElectroOpticalSensorStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        InfraredImagingSensorStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        SyntheticApertureRadarStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        SIGINTSensorStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        ELINTSensorStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        WeatherRadarStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        LaserDesignatorStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        MultiSpectralImagingStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        HyperspectralImagingStatus,

        [TelemetryCategory(TelemetryCategories.Surveillance)]
        CommunicationsRelayStatus,
    }
}
