using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class ChecksumEncoder : ITelemetryFieldEncoder
    {
        private static readonly Random _random = new Random();

        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.Checksum;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = ((SimulationConstants.TelemetryCompression.BIT_SHIFT_BASE << bits) - 1);
            return (ulong)_random.Next(0, (int)maxValue + 1);
        }
    }
} 