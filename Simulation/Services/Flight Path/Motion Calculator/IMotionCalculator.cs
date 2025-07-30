using Simulation.Common.Enums;
using Simulation.Models;

namespace Simulation.Services.Flight_Path.Motion_Calculator
{
    public interface IMotionCalculator
    {
        Location CalculateNext(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec
        );
    }
}
