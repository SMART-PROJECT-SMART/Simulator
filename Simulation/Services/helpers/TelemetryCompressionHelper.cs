using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers
{
    public static class TelemetryCompressionHelper
    {
        private static readonly Dictionary<TelemetryFields, int> _sizeInBits = new()
        {
            { TelemetryFields.DragCoefficient, SimulationConstants.TelemetryCompression.DRAG_COEFFICIENT_BITS },
            { TelemetryFields.LiftCoefficient, SimulationConstants.TelemetryCompression.LIFT_COEFFICIENT_BITS },
            { TelemetryFields.ThrottlePercent, SimulationConstants.TelemetryCompression.THROTTLE_PERCENT_BITS },
            { TelemetryFields.CruiseAltitude, SimulationConstants.TelemetryCompression.CRUISE_ALTITUDE_BITS },
            { TelemetryFields.Latitude, SimulationConstants.TelemetryCompression.LATITUDE_BITS },
            { TelemetryFields.LandingGearStatus, SimulationConstants.TelemetryCompression.LANDING_GEAR_STATUS_BITS },
            { TelemetryFields.Longitude, SimulationConstants.TelemetryCompression.LONGITUDE_BITS },
            { TelemetryFields.Altitude, SimulationConstants.TelemetryCompression.ALTITUDE_BITS },
            { TelemetryFields.CurrentSpeedKmph, SimulationConstants.TelemetryCompression.CURRENT_SPEED_KMPH_BITS },
            { TelemetryFields.YawDeg, SimulationConstants.TelemetryCompression.YAW_DEG_BITS },
            { TelemetryFields.PitchDeg, SimulationConstants.TelemetryCompression.PITCH_DEG_BITS },
            { TelemetryFields.RollDeg, SimulationConstants.TelemetryCompression.ROLL_DEG_BITS },
            { TelemetryFields.ThrustAfterInfluence, SimulationConstants.TelemetryCompression.THRUST_AFTER_INFLUENCE_BITS },
            { TelemetryFields.FuelAmount, SimulationConstants.TelemetryCompression.FUEL_AMOUNT_BITS },
            { TelemetryFields.DataStorageUsedGB, SimulationConstants.TelemetryCompression.DATA_STORAGE_USED_GB_BITS },
            { TelemetryFields.FlightTimeSec, SimulationConstants.TelemetryCompression.FLIGHT_TIME_SEC_BITS },
            { TelemetryFields.SignalStrength, SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_BITS },
            { TelemetryFields.Rpm, SimulationConstants.TelemetryCompression.RPM_BITS },
            { TelemetryFields.EngineDegrees, SimulationConstants.TelemetryCompression.ENGINE_DEGREES_BITS },
            { TelemetryFields.NearestSleeveId, SimulationConstants.TelemetryCompression.NEAREST_SLEEVE_ID_BITS },
            { TelemetryFields.Checksum, SimulationConstants.TelemetryCompression.CHECKSUM_BITS },
        };

        public static byte[] CompressTelemetryData(Dictionary<TelemetryFields, double> telemetryData)
        {
            int totalDataBits = _sizeInBits.Where(kvp => kvp.Key != TelemetryFields.Checksum)
                                           .Sum(kvp => kvp.Value);
            int totalBits = totalDataBits + _sizeInBits[TelemetryFields.Checksum];
            
            int totalBytes = (totalBits + SimulationConstants.TelemetryCompression.BITS_PER_BYTE - 1) / SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
            byte[] result = new byte[totalBytes];

            int bitOffset = 0;

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                if (field == TelemetryFields.Checksum)
                    continue;

                double value = telemetryData.GetValueOrDefault(field, 0.0);
                int bits = _sizeInBits[field];
                ulong encodedValue = EncodeFieldValueWithPrecision(field, value, bits);

                WriteBitsToBuffer(result, bitOffset, encodedValue, bits);
                bitOffset += bits;
            }

            uint checksum = CalculateSimpleChecksum(result, totalDataBits);
            
            WriteBitsToBuffer(result, bitOffset, checksum, _sizeInBits[TelemetryFields.Checksum]);

            return result;
        }

        public static Dictionary<TelemetryFields, double> DecompressTelemetryData(byte[] compressedData)
        {
            Dictionary<TelemetryFields, double> result = new();
            int bitOffset = 0;

            int totalDataBits = _sizeInBits.Where(kvp => kvp.Key != TelemetryFields.Checksum)
                                           .Sum(kvp => kvp.Value);

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                if (field == TelemetryFields.Checksum)
                    continue;

                int bits = _sizeInBits[field];
                ulong encodedValue = ReadBitsFromBuffer(compressedData, bitOffset, bits);
                double value = DecodeFieldValueWithPrecision(field, encodedValue);

                result[field] = value;
                bitOffset += bits;
            }

            uint storedChecksum = (uint)ReadBitsFromBuffer(compressedData, bitOffset, _sizeInBits[TelemetryFields.Checksum]);
            uint calculatedChecksum = CalculateSimpleChecksum(compressedData, totalDataBits);

            if (storedChecksum != calculatedChecksum)
            {
                throw new InvalidDataException($"Checksum validation failed. Expected: {calculatedChecksum:X8}, Got: {storedChecksum:X8}");
            }

            result[TelemetryFields.Checksum] = storedChecksum;
            return result;
        }

        private static ulong EncodeFieldValueWithPrecision(TelemetryFields field, double value, int bits)
        {
            ulong maxValue = ((SimulationConstants.TelemetryCompression.BOOLEAN_TRUE_VALUE << bits) - 1);

            return field switch
            {
                TelemetryFields.DragCoefficient or TelemetryFields.LiftCoefficient =>
                    (ulong)Math.Clamp(Math.Round(value * SimulationConstants.TelemetryCompression.PRECISION_SCALE_FACTOR), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.ThrottlePercent or TelemetryFields.FuelAmount =>
                    (ulong)Math.Clamp(Math.Round(value), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.CruiseAltitude or TelemetryFields.Altitude =>
                    (ulong)Math.Clamp(Math.Round(value), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.Latitude =>
                    (ulong)Math.Clamp(Math.Round((value + SimulationConstants.TelemetryCompression.LATITUDE_OFFSET) * SimulationConstants.TelemetryCompression.COORDINATE_SCALE), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),
                TelemetryFields.Longitude =>
                    (ulong)Math.Clamp(Math.Round((value + SimulationConstants.TelemetryCompression.LONGITUDE_OFFSET) * SimulationConstants.TelemetryCompression.COORDINATE_SCALE), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.LandingGearStatus =>
                    value > SimulationConstants.TelemetryData.WHEELS_UP ? SimulationConstants.TelemetryCompression.BOOLEAN_TRUE_VALUE : SimulationConstants.TelemetryCompression.BOOLEAN_FALSE_VALUE,

                TelemetryFields.CurrentSpeedKmph =>
                    (ulong)Math.Clamp(Math.Round(value), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.YawDeg or TelemetryFields.EngineDegrees =>
                    (ulong)Math.Clamp(Math.Round(value * SimulationConstants.TelemetryCompression.ANGLE_SCALE), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),
                TelemetryFields.PitchDeg or TelemetryFields.RollDeg =>
                    (ulong)Math.Clamp(Math.Round((value + SimulationConstants.TelemetryCompression.ANGLE_OFFSET) * SimulationConstants.TelemetryCompression.ANGLE_SCALE), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.ThrustAfterInfluence =>
                    (ulong)Math.Clamp(Math.Round(value * SimulationConstants.TelemetryCompression.PRECISION_SCALE_FACTOR), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.DataStorageUsedGB =>
                    (ulong)Math.Clamp(Math.Round(value * SimulationConstants.TelemetryCompression.PERCENTAGE_SCALE), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.FlightTimeSec =>
                    (ulong)Math.Clamp(Math.Round(value), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.SignalStrength =>
                    (ulong)Math.Clamp(Math.Round((value + SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_OFFSET) * SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_SCALE), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.Rpm =>
                    (ulong)Math.Clamp(Math.Round(value), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                TelemetryFields.NearestSleeveId =>
                    (ulong)Math.Clamp(Math.Round(value), SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE, maxValue),

                _ => throw new ArgumentException($"Unknown telemetry field: {field}")
            };
        }

        private static double DecodeFieldValueWithPrecision(TelemetryFields field, ulong encodedValue)
        {
            return field switch
            {
                TelemetryFields.DragCoefficient or TelemetryFields.LiftCoefficient =>
                    (double)encodedValue / SimulationConstants.TelemetryCompression.PRECISION_SCALE_FACTOR,

                TelemetryFields.ThrottlePercent or TelemetryFields.FuelAmount =>
                    encodedValue,

                TelemetryFields.CruiseAltitude or TelemetryFields.Altitude =>
                    encodedValue,

                TelemetryFields.Latitude =>
                    ((double)encodedValue / SimulationConstants.TelemetryCompression.COORDINATE_SCALE) - SimulationConstants.TelemetryCompression.LATITUDE_OFFSET,
                TelemetryFields.Longitude =>
                    ((double)encodedValue / SimulationConstants.TelemetryCompression.COORDINATE_SCALE) - SimulationConstants.TelemetryCompression.LONGITUDE_OFFSET,

                TelemetryFields.LandingGearStatus =>
                    encodedValue > SimulationConstants.TelemetryCompression.BOOLEAN_FALSE_VALUE ? SimulationConstants.TelemetryData.WHEELS_DOWN : SimulationConstants.TelemetryData.WHEELS_UP,

                TelemetryFields.CurrentSpeedKmph =>
                    encodedValue,

                TelemetryFields.YawDeg or TelemetryFields.EngineDegrees =>
                    (double)encodedValue / SimulationConstants.TelemetryCompression.ANGLE_SCALE,
                TelemetryFields.PitchDeg or TelemetryFields.RollDeg =>
                    ((double)encodedValue / SimulationConstants.TelemetryCompression.ANGLE_SCALE) - SimulationConstants.TelemetryCompression.ANGLE_OFFSET,

                TelemetryFields.ThrustAfterInfluence =>
                    (double)encodedValue / SimulationConstants.TelemetryCompression.PRECISION_SCALE_FACTOR,

                TelemetryFields.DataStorageUsedGB =>
                    (double)encodedValue / SimulationConstants.TelemetryCompression.PERCENTAGE_SCALE,

                TelemetryFields.FlightTimeSec =>
                    encodedValue,

                TelemetryFields.SignalStrength =>
                    ((double)encodedValue / SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_SCALE) - SimulationConstants.TelemetryCompression.SIGNAL_STRENGTH_OFFSET,

                TelemetryFields.Rpm =>
                    encodedValue,

                TelemetryFields.NearestSleeveId =>
                    encodedValue,

                _ => throw new ArgumentException($"Unknown telemetry field: {field}")
            };
        }

        private static void WriteBitsToBuffer(byte[] buffer, int bitOffset, ulong value, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                int byteIndex = (bitOffset + i) / SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
                int bitIndex = (bitOffset + i) % SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
                
                if ((value & (1UL << i)) != 0)
                {
                    buffer[byteIndex] |= (byte)(SimulationConstants.TelemetryCompression.BIT_MASK_SINGLE << bitIndex);
                }
            }
        }

        private static ulong ReadBitsFromBuffer(byte[] buffer, int bitOffset, int bitCount)
        {
            ulong result = 0;
            
            for (int i = 0; i < bitCount; i++)
            {
                int byteIndex = (bitOffset + i) / SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
                int bitIndex = (bitOffset + i) % SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
                
                if ((buffer[byteIndex] & (SimulationConstants.TelemetryCompression.BIT_MASK_SINGLE << bitIndex)) != 0)
                {
                    result |= (1UL << i);
                }
            }
            
            return result;
        }

        private static uint CalculateSimpleChecksum(byte[] data, int totalDataBits)
        {
            uint checksum = SimulationConstants.TelemetryCompression.CHECKSUM_SEED;
            int dataBytes = (totalDataBits + SimulationConstants.TelemetryCompression.BITS_PER_BYTE - 1) / SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
            
            for (int i = 0; i < dataBytes; i++)
            {
                checksum = ((checksum * SimulationConstants.TelemetryCompression.CHECKSUM_MULTIPLIER) + data[i] + SimulationConstants.TelemetryCompression.CHECKSUM_INCREMENT) & SimulationConstants.TelemetryCompression.CHECKSUM_MODULO;
            }
            
            int remainingBits = totalDataBits % SimulationConstants.TelemetryCompression.BITS_PER_BYTE;
            if (remainingBits > 0 && dataBytes > 0)
            {
                byte lastByte = (byte)(data[dataBytes - 1] & ((SimulationConstants.TelemetryCompression.BIT_MASK_SINGLE << remainingBits) - 1));
                checksum = ((checksum * SimulationConstants.TelemetryCompression.CHECKSUM_MULTIPLIER) + lastByte + SimulationConstants.TelemetryCompression.CHECKSUM_INCREMENT) & SimulationConstants.TelemetryCompression.CHECKSUM_MODULO;
            }
            
            return checksum;
        }

        public static bool ValidateChecksum(byte[] compressedData)
        {
            try
            {
                int totalDataBits = _sizeInBits.Where(kvp => kvp.Key != TelemetryFields.Checksum)
                                               .Sum(kvp => kvp.Value);
                
                uint storedChecksum = (uint)ReadBitsFromBuffer(compressedData, totalDataBits, _sizeInBits[TelemetryFields.Checksum]);
                uint calculatedChecksum = CalculateSimpleChecksum(compressedData, totalDataBits);
                
                return storedChecksum == calculatedChecksum;
            }
            catch
            {
                return false;
            }
        }
    }
}
