using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers.TelemetryCompression.Encoders
{
    public class LandingGearEncoder : ITelemetryFieldEncoder
    {
        public bool CanHandle(TelemetryFields field) => field == TelemetryFields.LandingGearStatus;

        public ulong Encode(double value, int bits)
        {
            return value > SimulationConstants.TelemetryData.WHEELS_UP
                ? SimulationConstants.TelemetryCompression.BOOLEAN_TRUE_VALUE
                : SimulationConstants.TelemetryCompression.BOOLEAN_FALSE_VALUE;
        }
    }
}
