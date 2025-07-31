using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.helpers;

public static class FlightPhysicsCalculator
{
    public static double CalculateDrag(Dictionary<TelemetryFields, double> telemetry,double frontalSurface)
    {
        telemetry.TryGetValue(TelemetryFields.DragCoefficient, out double dragCoefficient);
        telemetry.TryGetValue(TelemetryFields.CurrentSpeedKmph, out double currentSpeedKmph);
        double horizontalSpeed = currentSpeedKmph.ToKmhFromMps();

        return 0.5
            * SimulationConstants.Mathematical.RHO
            * frontalSurface
            * dragCoefficient
            * Math.Pow(horizontalSpeed, 2);
    }

    public static double CalculateLift(Dictionary<TelemetryFields, double> telemetry,double wingSurface)
    {
        telemetry.TryGetValue(TelemetryFields.LiftCoefficient, out double liftCoefficient);
        double currentSpeedKmph = telemetry.GetValueOrDefault(
            TelemetryFields.CurrentSpeedKmph,
            0.0
        );
        double horizontalSpeed = currentSpeedKmph.ToKmhFromMps();
        return 0.5
            * SimulationConstants.Mathematical.RHO
            * wingSurface
            * liftCoefficient
            * Math.Pow(horizontalSpeed, 2);
    }

    public static double CalculateThrust(Dictionary<TelemetryFields,
        double> telemetry,
        double thrustMax,
        double maxCruiseSpeed)
    {
        CalculateThrottle(telemetry,maxCruiseSpeed);

        double throttle = telemetry.GetValueOrDefault(TelemetryFields.ThrottlePercent, 0.0) / 100;
        double thrustAfterInfluence = telemetry.GetValueOrDefault(
            TelemetryFields.ThrustAfterInfluence,
            thrustMax
        );
        telemetry.TryGetValue(TelemetryFields.Altitude, out double altitude);
        double rawThrust = Math.Min(thrustMax * throttle, thrustAfterInfluence);
        double densityRatio = Math.Exp(
            -altitude / SimulationConstants.FlightPath.EARTH_SCALE_HEIGHT
        );

        return rawThrust * densityRatio;
    }

    public static double CalculateAcceleration(Dictionary<TelemetryFields,
        double> telemetry,
        double thrustMax,
        double frontalSurface,
        double mass,
        double maxAcceleration,
        double maxCruiseSpeed)
    {
        double thrust = CalculateThrust(telemetry,thrustMax,maxCruiseSpeed);
        double drag = CalculateDrag(telemetry,frontalSurface);

        double physicsAcceleration = (thrust - drag) / mass;

        if (physicsAcceleration < SimulationConstants.Mathematical.MIN_ACCELERATION_FACTOR)
        {
            double thrustToWeightRatio =
                thrust / (mass * SimulationConstants.FlightPath.GRAVITY_MPS2);
            double realisticAcceleration =
                maxAcceleration * Math.Min(thrustToWeightRatio * 2.0, 1.0);
            return Math.Max(realisticAcceleration, 0.5);
        }

        return Math.Min(physicsAcceleration, maxAcceleration);
    }

    private static void CalculateThrottle(Dictionary<TelemetryFields, double> telemetry,double maxCruiseSpeed)
    {
        double currentSpeed = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);

        double speedError = maxCruiseSpeed - currentSpeed;
        double throttlePercent = 100.0 * Math.Clamp(speedError / maxCruiseSpeed, 0.0, 1.0);
        telemetry[TelemetryFields.ThrottlePercent] = throttlePercent;
    }

    public static double CalculateAltitudeChange(
        double travelM,
        double pitchDeg,
        double deltaSec,
        Dictionary<TelemetryFields, double> telemetry,
        double altitude,
        double wingSurface,
        double frontalSurface
    )
    {
        double currentSpeedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
        double horizontalSpeedMps = currentSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
        double pitchRad = pitchDeg.ToRadians();
        
        double verticalVelocityMps = horizontalSpeedMps * Math.Tan(pitchRad);
        
        double altChange = verticalVelocityMps * deltaSec;

        if (Math.Abs(pitchDeg) > SimulationConstants.Mathematical.EPSILON && Math.Abs(altChange) > 0.1)
        {
            double lift = CalculateLift(telemetry,wingSurface);
            double liftContribution = CalculateLiftContribution(lift, deltaSec);
            altChange += liftContribution;

            double drag = CalculateDrag(telemetry, frontalSurface);
            double dragEffect = CalculateDragEffect(drag, deltaSec);
            altChange += dragEffect;
        }

        return altitude + altChange;
    }

    private static double CalculateLiftContribution(double lift, double deltaSec) =>
        (lift * deltaSec).FromMToKm();

    private static double CalculateDragEffect(double drag, double deltaSec) =>
        -drag * deltaSec * SimulationConstants.FlightPath.DRAG_EFFECT_ON_ALTITUDE;
}
