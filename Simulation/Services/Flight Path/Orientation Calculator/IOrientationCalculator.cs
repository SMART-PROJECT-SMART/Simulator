using Simulation.Models.Mission;

namespace Simulation.Services.Flight_Path.Orientation_Calculator
{
    public interface IOrientationCalculator
    {
        (double yaw, double pitch, double roll) ComputeOrientation(
            Location previous,
            Location current,
            double speedKmph,
            double deltaSec);
            
        void Reset();
    }
}