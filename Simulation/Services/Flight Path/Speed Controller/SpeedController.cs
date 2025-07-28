namespace Simulation.Services.Flight_Path.Speed_Controller;

public class SpeedController : ISpeedController
{
    public double ComputeNextSpeed(
        double currentSpeedKmph,
        double remainingKm,
        double maxAccelMps2,
        double maxDecelMps2,
        double deltaSec,
        double maxCruiseKmph)
    {
        double speedMps = currentSpeedKmph * 1000.0 / 3600.0;
        double remainingMeters = remainingKm * 1000.0;

        double brakingDistance = (speedMps * speedMps) / (2 * maxDecelMps2);
        double accelMps2 = remainingMeters <= brakingDistance
            ? -maxDecelMps2
            : maxAccelMps2;

        double newSpeedMps = Math.Max(0.0, speedMps + accelMps2 * deltaSec);
        double newKmph = newSpeedMps * 3.6;

        return maxCruiseKmph > 0
            ? Math.Min(newKmph, maxCruiseKmph)
            : newKmph;
    }
}