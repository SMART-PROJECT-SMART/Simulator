using Simulation.Common.Enums;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class SleeveIdEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.NearestSleeveId;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = ((1UL << bits) - 1);
            return (ulong)Math.Clamp(Math.Round(value), 0, maxValue);
        }
    }
} 