using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.ICDModels;
using System.Collections;

namespace Simulation.Services.Helpers
{
    public static class TelemetryCompressionHelper
    {
        public static BitArray CompressTelemetryDataByICD(Dictionary<TelemetryFields, double> telemetryData, ICD icd)
        {
            BitArray compressed = new BitArray(icd.GetSizeInBites());
            BitArray signBits = new BitArray(icd.Document.Count);

            int fieldIndex = 0;
            foreach (ICDItem telemetryParameter in icd)
            {
                int startBit = telemetryParameter.StartBitArrayIndex;
                int bitLength = telemetryParameter.BitLength;
                
                ulong valueInBits = 0;
                
                if (telemetryData.TryGetValue(telemetryParameter.Name, out double telemetryValue))
                {
                    bool isNegative = telemetryValue < 0;
                    signBits[fieldIndex] = isNegative;
                    
                    byte[] doubleBytes = BitConverter.GetBytes(telemetryValue);
                    ulong doubleBits = BitConverter.ToUInt64(doubleBytes, 0);
                    valueInBits = doubleBits;
                }
                else
                {
                    signBits[fieldIndex] = false;
                }
                
                for (int offset = 0; offset < bitLength; offset++)
                {
                    compressed[startBit + offset] = GetValueByOffset(valueInBits, offset);
                }
                
                fieldIndex++;
            }
            
            BitArray compressedWithSigns = AppendSignBits(compressed, signBits);
            
            uint checksum = CalculateChecksum(compressedWithSigns);
            return AppendChecksum(compressedWithSigns, checksum);
        }

        private static BitArray AppendSignBits(BitArray data, BitArray signBits)
        {
            if (signBits.Length == 0)
                return data;
                
            var result = new BitArray(data.Length + signBits.Length);
            
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }
            
            for (int i = 0; i < signBits.Length; i++)
            {
                result[data.Length + i] = signBits[i];
            }
            
            return result;
        }

        private static bool GetValueByOffset(ulong value, int offset)
        {
            return (value & (1UL << offset)) != 0;
        }

        private static uint CalculateChecksum(BitArray data)
        {
            uint checksum = SimulationConstants.TelemetryCompression.CHECKSUM_SEED;

            int byteCount = (data.Length + SimulationConstants.Networking.BYTE_SIZE - 1) 
                          / SimulationConstants.Networking.BYTE_SIZE;

            for (int byteIndex = 0; byteIndex < byteCount; byteIndex++)
            {
                byte dataByte = GetByteValue(data, byteIndex);

                checksum = ((checksum * SimulationConstants.TelemetryCompression.CHECKSUM_MULTIPLIER
                           + SimulationConstants.TelemetryCompression.CHECKSUM_INCREMENT
                           + dataByte) & SimulationConstants.TelemetryCompression.CHECKSUM_MODULO);
            }

            return checksum;
        }

        private static byte GetByteValue(BitArray data, int byteIndex)
        {
            byte dataByte = 0;
            int bitsInThisByte = Math.Min(
                SimulationConstants.Networking.BYTE_SIZE,
                data.Length - (byteIndex * SimulationConstants.Networking.BYTE_SIZE)
            );
            for (int bitIndex = 0; bitIndex < bitsInThisByte; bitIndex++)
            {
                int absoluteBitIndex = (byteIndex * SimulationConstants.Networking.BYTE_SIZE) + bitIndex;
                if (absoluteBitIndex < data.Length && data[absoluteBitIndex])
                {
                    dataByte |= (byte)(1 << bitIndex);
                }
            }
            return dataByte;
        }

        private static BitArray AppendChecksum(BitArray data, uint checksum)
        {
            int checksumBits = SimulationConstants.TelemetryCompression.CHECKSUM_BITS;
            var result = new BitArray(data.Length + checksumBits);

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }

            for (int i = 0; i < checksumBits; i++)
            {
                result[data.Length + i] = (checksum & (SimulationConstants.TelemetryCompression.BIT_SHIFT_BASE << i)) != 0;
            }

            return result;
        }
    }
}
