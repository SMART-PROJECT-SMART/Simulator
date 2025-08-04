using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class LandingGearEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.LandingGearStatus;

        public ulong Encode(double value, int bits)
        {
            return value > SimulationConstants.TelemetryData.WHEELS_UP ? 1UL : 0UL;
        }
    }
} 