using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class OffsetAngleEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) =>
            field == TelemetryFields.PitchDeg || field == TelemetryFields.RollDeg;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = (
                (SimulationConstants.TelemetryCompression.BIT_SHIFT_BASE << bits) - 1
            );
            return (ulong)
                Math.Clamp(
                    Math.Round(
                        (value + SimulationConstants.TelemetryCompression.ANGLE_OFFSET)
                            * SimulationConstants.TelemetryCompression.ANGLE_SCALE
                    ),
                    0,
                    maxValue
                );
        }
    }
}
