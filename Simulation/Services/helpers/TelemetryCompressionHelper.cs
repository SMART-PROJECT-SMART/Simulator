using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers
{
    public static class TelemetryCompressionHelper
    {
        private static readonly Dictionary<TelemetryFields, int> _sizeInBytes = new()
        {
            { TelemetryFields.DragCoefficient, 2 },
            { TelemetryFields.LiftCoefficient, 2 },
            { TelemetryFields.ThrottlePercent, 1 },
            { TelemetryFields.CruiseAltitude, 4 },
            { TelemetryFields.Latitude, 4 },
            { TelemetryFields.LandingGearStatus, 1 },
            { TelemetryFields.Longitude, 4 },
            { TelemetryFields.Altitude, 4 },
            { TelemetryFields.CurrentSpeedKmph, 2 },
            { TelemetryFields.YawDeg, 2 },
            { TelemetryFields.PitchDeg, 2 },
            { TelemetryFields.RollDeg, 2 },
            { TelemetryFields.ThrustAfterInfluence, 2 },
            { TelemetryFields.FuelAmount, 2 },
            { TelemetryFields.DataStorageUsedGB, 2 },
            { TelemetryFields.FlightTimeSec, 4 },
            { TelemetryFields.SignalStrength, 2 },
            { TelemetryFields.Rpm, 2 },
            { TelemetryFields.EngineDegrees, 2 },
            { TelemetryFields.Checksum, 4 },
        };

        private static readonly uint[] _crc32Table = GenerateCrc32Table();

        public static byte[] CompressTelemetryData(
            Dictionary<TelemetryFields, double> telemetryData
        )
        {
            int dataSize = _sizeInBytes.Where(kvp => kvp.Key != TelemetryFields.Checksum).Sum(kvp => kvp.Value);
            int totalSize = _sizeInBytes.Values.Sum();
            byte[] result = new byte[totalSize];
            int offset = 0;

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                if (field == TelemetryFields.Checksum)
                    continue;

                double value = telemetryData.GetValueOrDefault(field, 0.0);
                int size = _sizeInBytes[field];

                byte[] fieldBytes = size switch
                {
                    1 => new[] { (byte)value },
                    2 => BitConverter.GetBytes((short)value),
                    4 => BitConverter.GetBytes((float)value),
                    8 => BitConverter.GetBytes(value),
                    _ => throw new NotSupportedException(),
                };

                Buffer.BlockCopy(fieldBytes, 0, result, offset, size);
                offset += size;
            }

            uint checksum = CalculateChecksum(result, 0, dataSize);
            
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            Buffer.BlockCopy(checksumBytes, 0, result, offset, 4);

            return result;
        }

        public static Dictionary<TelemetryFields, double> DecompressTelemetryData(
            byte[] compressedData
        )
        {
            Dictionary<TelemetryFields, double> result = new();
            int offset = 0;

            int dataSize = _sizeInBytes.Where(kvp => kvp.Key != TelemetryFields.Checksum).Sum(kvp => kvp.Value);
            
            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                if (field == TelemetryFields.Checksum)
                    continue;

                int size = _sizeInBytes[field];
                double value = size switch
                {
                    1 => compressedData[offset],
                    2 => BitConverter.ToInt16(compressedData, offset),
                    4 => BitConverter.ToSingle(compressedData, offset),
                    8 => BitConverter.ToDouble(compressedData, offset),
                    _ => throw new NotSupportedException(),
                };

                result[field] = value;
                offset += size;
            }

            uint storedChecksum = BitConverter.ToUInt32(compressedData, offset);
            uint calculatedChecksum = CalculateChecksum(compressedData, 0, dataSize);
            
            if (storedChecksum != calculatedChecksum)
            {
                throw new InvalidDataException($"Checksum validation failed. Expected: {calculatedChecksum}, Got: {storedChecksum}");
            }

            result[TelemetryFields.Checksum] = storedChecksum;

            return result;
        }

        private static uint CalculateChecksum(byte[] data, int offset, int length)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = offset; i < offset + length; i++)
            {
                crc = _crc32Table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
            }
            return crc ^ 0xFFFFFFFF;
        }

        private static uint[] GenerateCrc32Table()
        {
            uint[] table = new uint[256];
            const uint polynomial = 0xEDB88320;

            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                table[i] = crc;
            }
            return table;
        }

        public static bool ValidateChecksum(byte[] compressedData)
        {
            try
            {
                int dataSize = _sizeInBytes.Where(kvp => kvp.Key != TelemetryFields.Checksum).Sum(kvp => kvp.Value);
                uint storedChecksum = BitConverter.ToUInt32(compressedData, dataSize);
                uint calculatedChecksum = CalculateChecksum(compressedData, 0, dataSize);
                return storedChecksum == calculatedChecksum;
            }
            catch
            {
                return false;
            }
        }
    }
}
