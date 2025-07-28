namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public interface ISpeedController
    {
        double ComputeNextSpeed(
            double currentSpeedKmph,
            double remainingKm,
            double maxAccelMps2,
            double maxDecelMps2,
            double deltaSec,
            double maxCruiseKmph);
    }
}