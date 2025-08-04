using System.Collections;
using Simulation.Common.Enums;
using Simulation.Models.ICD;
using Simulation.Services.Helpers.TelemetryCompression;
using Simulation.Services.Helpers.TelemetryCompression.Strategies;

namespace Simulation.Services.Helpers
{
    public static class TelemetryCompressionHelper
    {
        private static readonly ITelemetryCompressionStrategy _compressionStrategy;

        static TelemetryCompressionHelper()
        {
            _compressionStrategy = new BitPackingCompressionStrategy();
        }

        public static BitArray CompressTelemetryData(Models.ICD.ICD icd)
        {
            var telemetryData = icd.Document.ToDictionary(item => item.Name, item => item.Value);
            return _compressionStrategy.Compress(telemetryData);
        }

        public static BitArray CompressTelemetryData(Dictionary<TelemetryFields, double> telemetryData)
        {
            return _compressionStrategy.Compress(telemetryData);
        }
    }
}
