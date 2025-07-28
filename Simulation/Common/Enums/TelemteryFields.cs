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
        VelocityX,
        VelocityY,
        VelocityZ,

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

