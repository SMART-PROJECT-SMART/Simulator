using Simulation.Common.constants;
using System;

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
        double targetSpeedKmph = CalculateOptimalApproachSpeed(
            remainingKm,
            maxCruiseSpeedKmph);

        double speedDifference = targetSpeedKmph - currentSpeedKmph;
        double effectiveAccelDecel = speedDifference > 0
            ? maxAcceleration * 0.7
            : Math.Abs(maxDeceleration) * 0.8;

        double maxSpeedChangeKmph = effectiveAccelDecel * deltaSeconds * 3.6;
        double actualSpeedChange = Math.Sign(speedDifference)
            * Math.Min(Math.Abs(speedDifference), maxSpeedChangeKmph);

        if (Math.Abs(actualSpeedChange) > 5.0)
            actualSpeedChange = Math.Sign(actualSpeedChange) * 5.0;

        double newSpeed = currentSpeedKmph + actualSpeedChange;

        if (Math.Abs(actualSpeedChange) > 0.1)
        {
            string phase = speedDifference > 0 ? "ACCEL" : "DECEL";
            double stoppingDistance = CalculateStoppingDistance(
                currentSpeedKmph,
                maxDeceleration,
                3.0);

            Console.WriteLine(
                $"Speed: {currentSpeedKmph:F1} → {newSpeed:F1} km/h " +
                $"({phase} Δ{actualSpeedChange:F1}, " +
                $"Target: {targetSpeedKmph:F1}, " +
                $"Remaining: {remainingKm * 1000:F0}m, " +
                $"Stop needed: {stoppingDistance:F0}m)");
        }

        return Math.Max(1.0, newSpeed);
    }

    private static double CalculateOptimalApproachSpeed(
        double remainingKm,
        double maxCruiseSpeedKmph)
    {
        double finalLandingSpeedKmph = 16.67;
        double approachDistanceKm = 1.0;

        double normalized = Math.Max(0.0,
            Math.Min(1.0, remainingKm / approachDistanceKm));

        double speed = finalLandingSpeedKmph
            + (maxCruiseSpeedKmph - finalLandingSpeedKmph) * normalized;

        return Math.Max(finalLandingSpeedKmph,
            Math.Min(maxCruiseSpeedKmph, speed));
    }

    private static double CalculateStoppingDistance(
        double currentSpeedKmph,
        double maxDeceleration,
        double finalSpeedKmph)
    {
        double currentSpeedMps = currentSpeedKmph / 3.6;
        double finalSpeedMps = finalSpeedKmph / 3.6;

        double stoppingDistance = (currentSpeedMps * currentSpeedMps
            - finalSpeedMps * finalSpeedMps)
            / (2 * maxDeceleration);

        return Math.Max(0, stoppingDistance);
    }
}
