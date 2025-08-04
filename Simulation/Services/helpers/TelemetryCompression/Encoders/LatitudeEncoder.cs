using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class LatitudeEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.Latitude;

        public ulong Encode(double value, int bits)
        {
            ulong maxValue = ((1UL << bits) - 1);
            return (ulong)Math.Clamp(Math.Round((value + SimulationConstants.TelemetryCompression.LATITUDE_OFFSET) * SimulationConstants.TelemetryCompression.COORDINATE_SCALE), 0, maxValue);
        }
    }
} 