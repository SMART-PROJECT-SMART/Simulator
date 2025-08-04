using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class CoefficientEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => 
            field == TelemetryFields.DragCoefficient || field == TelemetryFields.LiftCoefficient;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = ((1UL << bits) - 1);
            return (ulong)Math.Clamp(Math.Round(value * SimulationConstants.TelemetryCompression.PRECISION_SCALE_FACTOR), 0, maxValue);
        }
    }
} 