using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class SignalStrengthEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.SignalStrength;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = ((1UL << bits) - 1);
            return (ulong)Math.Clamp(Math.Round((value + SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_OFFSET) * SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_SCALE), 0, maxValue);
        }
    }
} 