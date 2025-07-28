using Simulation.Common.constants;

namespace Simulation.Services.Flight_Path.Speed_Controller;

public class SpeedController : ISpeedController
{
    public double ComputeNextSpeed(
        double currentSpeedKmph,
        double remainingKm,
        double maxAcceleration,
        double maxDeceleration,
        double deltaSeconds,
        double maxCruiseSpeedKmph)
    {
        double targetSpeedKmph = remainingKm <= SimulationConstants.FlightPath.MIN_DESCENT_DISTANCE_KM
            ? Math.Max(SimulationConstants.FlightPath.MIN_SPEED_KMH * 2, remainingKm * 100)
            : maxCruiseSpeedKmph;

        double speedDifference = targetSpeedKmph - currentSpeedKmph;
        double maxSpeedChange = (speedDifference > 0 ? maxAcceleration : maxDeceleration) * deltaSeconds * 3.6;

        return currentSpeedKmph + Math.Sign(speedDifference) * Math.Min(Math.Abs(speedDifference), maxSpeedChange);
    }

    private static double CalculateBrakingDistance(double speedMps, double decelMps2)
    {
        return (speedMps * speedMps) / (2 * decelMps2);
    }
}