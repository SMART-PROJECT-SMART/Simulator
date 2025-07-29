namespace Simulation.Common.Enums
{
    public enum TelemetryFields
    {
        LocationLatitude,
        LocationLongitude,
        LocationAltitudeAmsl,
        LocationAltitudeAgl,
        LocationGroundSpeed,
        LocationVerticalSpeed,
        LocationYaw,
        LocationPitch,
        LocationRoll,
        LocationClimbRate,
        LocationDistanceFromHome,
        LocationDistanceToWaypoint,
        LocationWaypointIndex,

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
    }
}

