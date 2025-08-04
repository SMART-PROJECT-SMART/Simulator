using System.Collections;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers.TelemetryCompression
{
    public interface ITelemetryCompressionStrategy
    {
        BitArray Compress(Dictionary<TelemetryFields, double> telemetryData);
    }
} 