using Simulation.Models.Mission;

namespace Simulation.Services.Flight_Path.Motion_Calculator
{
    public interface IMotionCalculator
    {
        Location CalculateNext(
            Location current,
            Location destination,
            double groundSpeedMps,
            double pitchDegrees,
            double deltaSeconds);
    }
}
