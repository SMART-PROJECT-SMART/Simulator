using Simulation.Common.constants;
using Simulation.Common.Enums;

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
        double horizontalSpeed =
            currentSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;

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
        double horizontalSpeed =
            currentSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
        return 0.5
            * SimulationConstants.Mathematical.RHO
            * wingsSurface
            * liftCoefficient
            * Math.Pow(horizontalSpeed, 2);
    }

    public static double CalculateThrust(Dictionary<TelemetryFields, double> telemetry)
    {
        return telemetry.GetValueOrDefault(TelemetryFields.ThrustAfterInfluence, 0.0);
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

        if (physicsAcceleration < 0.1)
        {
            double thrustToWeightRatio =
                thrust / (mass * SimulationConstants.FlightPath.GRAVITY_MPS2);
            double realisticAcceleration =
                maxAcceleration * Math.Min(thrustToWeightRatio * 2.0, 1.0);
            return Math.Max(realisticAcceleration, 0.5);
        }

        return Math.Min(physicsAcceleration, maxAcceleration);
    }
}
