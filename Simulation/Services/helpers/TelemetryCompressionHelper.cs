using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.ICD;
using Simulation.Services.Helpers.TelemetryCompression;
using Simulation.Services.Helpers.TelemetryCompression.Strategies;
using System.Collections;

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
            BitArray compressed = _compressionStrategy.Compress(telemetryData);
            uint checksum = CalculateLinearCongruentialChecksum(compressed);
            return AppendChecksum(compressed, checksum);
        }
        private static uint CalculateLinearCongruentialChecksum(BitArray data)
        {
            uint checksum = SimulationConstants.TelemetryCompression.CHECKSUM_SEED;

            int byteCount = (data.Length + 7) / 8; 

            for (int byteIndex = 0; byteIndex < byteCount; byteIndex++)
            {
                byte dataByte = 0;

                int bitsInThisByte = Math.Min(8, data.Length - (byteIndex * 8));

                for (int bitIndex = 0; bitIndex < bitsInThisByte; bitIndex++)
                {
                    int absoluteBitIndex = (byteIndex * 8) + bitIndex;
                    if (absoluteBitIndex < data.Length && data[absoluteBitIndex])
                    {
                        dataByte |= (byte)(1 << bitIndex);
                    }
                }

                checksum = (checksum * SimulationConstants.TelemetryCompression.CHECKSUM_MULTIPLIER
                            + SimulationConstants.TelemetryCompression.CHECKSUM_INCREMENT
                            + dataByte) & SimulationConstants.TelemetryCompression.CHECKSUM_MODULO;
            }

            return checksum;
        }
        private static BitArray AppendChecksum(BitArray data, uint checksum)
        {
            const int checksumBits = SimulationConstants.TelemetryCompression.CHECKSUM_BITS;

            var result = new BitArray(data.Length + checksumBits);

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }

            for (int i = 0; i < checksumBits; i++)
            {
                result[data.Length + i] = (checksum & (1u << i)) != 0;
            }

            return result;
        }

        public static BitArray CompressTelemetryData(Dictionary<TelemetryFields, double> telemetryData)
        {
            return _compressionStrategy.Compress(telemetryData);
        }
    }
}
