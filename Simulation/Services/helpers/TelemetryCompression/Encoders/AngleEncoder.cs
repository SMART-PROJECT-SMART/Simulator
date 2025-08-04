using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class AngleEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => 
            field == TelemetryFields.YawDeg || field == TelemetryFields.EngineDegrees;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = ((1UL << bits) - 1);
            return (ulong)Math.Clamp(Math.Round(value * SimulationConstants.TelemetryCompression.ANGLE_SCALE), 0, maxValue);
        }
    }
} 