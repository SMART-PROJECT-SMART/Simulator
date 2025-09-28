using Core.Common.Enums;
using Simulation.Common.Enums;
using Simulation.Models;

namespace Simulation.Services.Flight_Path.Orientation_Calculator
{
    public interface IOrientationCalculator
    {
        AxisDegrees ComputeOrientation(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            Location destination,
            double deltaSec
        );
    }
}
