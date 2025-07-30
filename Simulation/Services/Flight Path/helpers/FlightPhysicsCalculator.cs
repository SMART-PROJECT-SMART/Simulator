using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.helpers;

public static class FlightPhysicsCalculator
{
    public static double CalculateDrag(Dictionary<TelemetryFields, double> telemetry)
    {
        double frontalSurface = telemetry.GetValueOrDefault(TelemetryFields.FrontalSurface, 1.0);
        double dragCoefficient = telemetry.GetValueOrDefault(TelemetryFields.DragCoefficient, 0.1);
        double currentSpeedKmph = telemetry.GetValueOrDefault(
            TelemetryFields.CurrentSpeedKmph,
            0.0
        );
        double horizontalSpeed = currentSpeedKmph.ToKmhFromMps();

        return 0.5
            * SimulationConstants.Mathematical.RHO
            * frontalSurface
            * dragCoefficient
            * Math.Pow(horizontalSpeed, 2);
    }

    public static double CalculateLift(Dictionary<TelemetryFields, double> telemetry)
    {
        double wingsSurface = telemetry.GetValueOrDefault(TelemetryFields.WingsSurface, 1.0);
        double liftCoefficient = telemetry.GetValueOrDefault(TelemetryFields.LiftCoefficient, 0.1);
        double currentSpeedKmph = telemetry.GetValueOrDefault(
            TelemetryFields.CurrentSpeedKmph,
            0.0
        );
        double horizontalSpeed = currentSpeedKmph.ToKmhFromMps();
        return 0.5
            * SimulationConstants.Mathematical.RHO
            * wingsSurface
            * liftCoefficient
            * Math.Pow(horizontalSpeed, 2);
    }

    public static double CalculateThrust(Dictionary<TelemetryFields, double> telemetry)
    {
        CalculateThrottle(telemetry);

        double throttle = telemetry.GetValueOrDefault(TelemetryFields.ThrottlePercent, 0.0) / 100;
        double thrustMax = telemetry.GetValueOrDefault(TelemetryFields.ThrustMax, 0.0);
        double thrustAfterInfluence = telemetry.GetValueOrDefault(
            TelemetryFields.ThrustAfterInfluence,
            thrustMax
        );
        double altitude = telemetry.GetValueOrDefault(TelemetryFields.Altitude, 0.0);
        double rawThrust = Math.Min(thrustMax * throttle, thrustAfterInfluence);
        double densityRatio = Math.Exp(
            -altitude / SimulationConstants.FlightPath.EARTH_SCALE_HEIGHT
        );

        return rawThrust * densityRatio;
    }

    public static double CalculateAcceleration(Dictionary<TelemetryFields, double> telemetry)
    {
        double thrust = CalculateThrust(telemetry);
        double drag = CalculateDrag(telemetry);
        double mass = telemetry.GetValueOrDefault(TelemetryFields.Mass, 1.0);
        double maxAcceleration = telemetry.GetValueOrDefault(
            TelemetryFields.MaxAccelerationMps2,
            2.0
        );

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

    private static void CalculateThrottle(Dictionary<TelemetryFields, double> telemetry)
    {
        double currentSpeed = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
        double cruiseSpeed = telemetry.GetValueOrDefault(TelemetryFields.MaxCruiseSpeedKmph, 0.0);

        double speedError = cruiseSpeed - currentSpeed;
        double throttlePercent = 100.0 * Math.Clamp(speedError / cruiseSpeed, 0.0, 1.0);
        telemetry[TelemetryFields.ThrottlePercent] = throttlePercent;
    }

    public static double CalculateAltitudeChange(
        double travelM,
        double pitchDeg,
        double deltaSec,
        Dictionary<TelemetryFields, double> telemetry,
        double altitude
    )
    {
        double currentSpeedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
        double horizontalSpeedMps = currentSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
        double pitchRad = UnitConversionHelper.ToRadians(pitchDeg);
        
        double verticalVelocityMps = horizontalSpeedMps * Math.Tan(pitchRad);
        
        double altChange = verticalVelocityMps * deltaSec;

        // Temporarily disabled lift/drag effects to fix altitude control issues
        // if (Math.Abs(pitchDeg) > SimulationConstants.Mathematical.EPSILON && Math.Abs(altChange) > 0.1)
        // {
        //     double lift = CalculateLift(telemetry);
        //     double liftContribution = CalculateLiftContribution(lift, deltaSec);
        //     altChange += liftContribution;

        //     double drag = CalculateDrag(telemetry);
        //     double dragEffect = CalculateDragEffect(drag, deltaSec);
        //     altChange += dragEffect;
        // }

        return altitude + altChange;
    }

    private static double CalculateLiftContribution(double lift, double deltaSec) =>
        (lift * deltaSec).FromMToKm();

    private static double CalculateDragEffect(double drag, double deltaSec) =>
        -drag * deltaSec * SimulationConstants.FlightPath.DRAG_EFFECT_ON_ALTITUDE;
}
