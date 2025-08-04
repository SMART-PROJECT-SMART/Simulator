using System.Collections;
using Simulation.Common.Enums;
using Simulation.Common.constants;
using Simulation.Services.Helpers.TelemetryCompression.Encoders;

namespace Simulation.Services.Helpers.TelemetryCompression.Strategies
{
    public class BitPackingCompressionStrategy : ITelemetryCompressionStrategy
    {
        private readonly Dictionary<TelemetryFields, int> _sizeInBits;
        private readonly Dictionary<TelemetryFields, ITelemetryFieldEncoder> _encoders;
        private readonly int _totalBits;
        private readonly TelemetryFields[] _dataFields;

        public BitPackingCompressionStrategy()
        {
            _sizeInBits = new Dictionary<TelemetryFields, int>
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
            };

            _encoders = new Dictionary<TelemetryFields, ITelemetryFieldEncoder>
            {
                { TelemetryFields.DragCoefficient, new CoefficientEncoder() },
                { TelemetryFields.LiftCoefficient, new CoefficientEncoder() },
                { TelemetryFields.ThrottlePercent, new PercentageEncoder() },
                { TelemetryFields.FuelAmount, new PercentageEncoder() },
                { TelemetryFields.CruiseAltitude, new AltitudeEncoder() },
                { TelemetryFields.Altitude, new AltitudeEncoder() },
                { TelemetryFields.Latitude, new LatitudeEncoder() },
                { TelemetryFields.Longitude, new LongitudeEncoder() },
                { TelemetryFields.LandingGearStatus, new LandingGearEncoder() },
                { TelemetryFields.CurrentSpeedKmph, new SpeedEncoder() },
                { TelemetryFields.YawDeg, new AngleEncoder() },
                { TelemetryFields.EngineDegrees, new AngleEncoder() },
                { TelemetryFields.PitchDeg, new OffsetAngleEncoder() },
                { TelemetryFields.RollDeg, new OffsetAngleEncoder() },
                { TelemetryFields.ThrustAfterInfluence, new ThrustEncoder() },
                { TelemetryFields.DataStorageUsedGB, new DataStorageEncoder() },
                { TelemetryFields.FlightTimeSec, new TimeEncoder() },
                { TelemetryFields.SignalStrength, new SignalStrengthEncoder() },
                { TelemetryFields.Rpm, new RpmEncoder() },
                { TelemetryFields.NearestSleeveId, new SleeveIdEncoder() },
            };

            _dataFields = Enum.GetValues<TelemetryFields>()
                .Where(field => field != TelemetryFields.Checksum)
                .ToArray();
            
            _totalBits = _dataFields.Sum(field => _sizeInBits[field]);
        }

        public BitArray Compress(Dictionary<TelemetryFields, double> telemetryData)
        {
            var result = new BitArray(_totalBits);
            int bitOffset = 0;

            foreach (var field in _dataFields)
            {
                double value = telemetryData.GetValueOrDefault(field, 0.0);
                int bits = _sizeInBits[field];
                ulong encodedValue = _encoders[field].Encode(value, bits);

                WriteBitsToArray(result, bitOffset, encodedValue, bits);
                bitOffset += bits;
            }

            return result;
        }

        private static void WriteBitsToArray(BitArray bitArray, int bitOffset, ulong value, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                bitArray[bitOffset + i] = (value & (1UL << i)) != 0;
            }
        }
    }
} 