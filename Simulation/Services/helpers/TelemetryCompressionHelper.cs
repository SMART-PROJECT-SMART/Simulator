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
            uint checksum = CalculateChecksum(compressed);
            return AppendChecksum(compressed, checksum);
        }
        private static uint CalculateChecksum(BitArray data)
        {
            uint checksum = SimulationConstants.TelemetryCompression.CHECKSUM_SEED;

            int byteCount = (data.Length + SimulationConstants.Networking.BYTE_SIZE -1) / SimulationConstants.Networking.BYTE_SIZE; 

            for (int byteIndex = 0; byteIndex < byteCount; byteIndex++)
            {
                byte dataByte = 0;

                int bitsInThisByte = Math.Min(SimulationConstants.Networking.BYTE_SIZE, data.Length - (byteIndex * SimulationConstants.Networking.BYTE_SIZE));

                for (int bitIndex = 0; bitIndex < bitsInThisByte; bitIndex++)
                {
                    int absoluteBitIndex = (byteIndex * SimulationConstants.Networking.BYTE_SIZE) + bitIndex;
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
                result[data.Length + i] = (checksum & (SimulationConstants.Networking.TRUE_VALUE << i)) != 0;
            }

            return result;
        }
    }
}
