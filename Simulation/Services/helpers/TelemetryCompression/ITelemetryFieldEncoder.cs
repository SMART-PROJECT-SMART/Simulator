using Simulation.Common.Enums;

namespace Simulation.Services.Helpers.TelemetryCompression
{
    public interface ITelemetryFieldEncoder
    {
        ulong Encode(double value, int bits);
        bool CanHandle(TelemetryFields field);
    }
}
