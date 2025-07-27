namespace Simulation.Common.Enums
{
    public enum TelemetryFields
    {
        PositionLatitude,
        PositionLongitude,
        PositionAltitudeAmsl,
        PositionAltitudeAgl,
        PositionGroundSpeed,
        PositionVerticalSpeed,
        PositionYaw,
        PositionPitch,
        PositionRoll,
        PositionClimbRate,
        PositionDistanceFromHome,
        PositionDistanceToWaypoint,
        PositionWaypointIndex,

        EngineRpm,
        EngineThrottlePercent,
        EngineFuelLevelPercent,
        EngineFuelConsumptionRate,
        EngineBatteryVoltage,
        EngineBatteryCurrent,
        EngineBatteryPercent,
        EngineMotorTemperature,
        EngineEscTemperature,
        EnginePowerDraw,

        EnvironmentAirspeed,
        EnvironmentAirTemperature,
        EnvironmentBarometricPressure,
        EnvironmentHumidity,
        EnvironmentWindSpeed,
        EnvironmentWindDirection,

        SystemUptimeSeconds,

        WeaponArmed,
        WeaponSelectedWeapon,
        WeaponAmmoAGM114,
        WeaponAmmo20mm
    }
}

