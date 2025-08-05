using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class SignalStrengthEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.SignalStrength;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = (
                (SimulationConstants.TelemetryCompression.BIT_SHIFT_BASE << bits) - 1
            );
            return (ulong)
                Math.Clamp(
                    Math.Round(
                        (value + SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_OFFSET)
                            * SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_SCALE
                    ),
                    0,
                    maxValue
                );
        }
    }
}
