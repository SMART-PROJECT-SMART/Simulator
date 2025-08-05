using Simulation.Common.Enums;
using Simulation.Common.constants;
using Simulation.Models.ICD;
using Simulation.Services.ICD.Interfaces;

namespace Simulation.Services.ICD.Factories
{
    public class TelemetryICDItemFactory : IICDItemFactory
    {
        private static readonly Random _random = new Random();
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
        };

        public ICDItem CreateItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return field switch
            {
                TelemetryFields.DragCoefficient or TelemetryFields.LiftCoefficient => CreateCoefficientItem(field, bitOffset, bitLength),
                TelemetryFields.ThrottlePercent or TelemetryFields.FuelAmount => CreatePercentageItem(field, bitOffset, bitLength),
                TelemetryFields.CruiseAltitude or TelemetryFields.Altitude => CreateAltitudeItem(field, bitOffset, bitLength),
                TelemetryFields.Latitude => CreateLatitudeItem(field, bitOffset, bitLength),
                TelemetryFields.Longitude => CreateLongitudeItem(field, bitOffset, bitLength),
                TelemetryFields.LandingGearStatus => CreateLandingGearItem(field, bitOffset, bitLength),
                TelemetryFields.CurrentSpeedKmph => CreateSpeedItem(field, bitOffset, bitLength),
                TelemetryFields.YawDeg => CreateYawItem(field, bitOffset, bitLength),
                TelemetryFields.PitchDeg or TelemetryFields.RollDeg => CreateOffsetAngleItem(field, bitOffset, bitLength),
                TelemetryFields.ThrustAfterInfluence => CreateThrustItem(field, bitOffset, bitLength),
                TelemetryFields.DataStorageUsedGB => CreateDataStorageItem(field, bitOffset, bitLength),
                TelemetryFields.FlightTimeSec => CreateTimeItem(field, bitOffset, bitLength),
                TelemetryFields.SignalStrength => CreateSignalStrengthItem(field, bitOffset, bitLength),
                TelemetryFields.Rpm => CreateRpmItem(field, bitOffset, bitLength),
                TelemetryFields.EngineDegrees => CreateEngineDegreesItem(field, bitOffset, bitLength),
                TelemetryFields.NearestSleeveId => CreateSleeveIdItem(field, bitOffset, bitLength),
                _ => throw new ArgumentException($"Unknown telemetry field: {field}"),
            };
        }

        private ICDItem CreateCoefficientItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.COEFFICIENT_MAX_VALUE),
                SimulationConstants.Units.COEFFICIENT,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.COEFFICIENT_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreatePercentageItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.PERCENTAGE_MAX_VALUE),
                SimulationConstants.Units.PERCENTAGE,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.PERCENTAGE_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateAltitudeItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ALTITUDE_MAX_VALUE),
                SimulationConstants.Units.METERS,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.ALTITUDE_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateLatitudeItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MIN, SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MAX),
                SimulationConstants.Units.DEGREES,
                SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MIN,
                SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MAX,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateLongitudeItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MIN, SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MAX),
                SimulationConstants.Units.DEGREES,
                SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MIN,
                SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MAX,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateLandingGearItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.TelemetryData.WHEELS_UP, SimulationConstants.TelemetryData.WHEELS_DOWN),
                SimulationConstants.Units.BOOLEAN,
                SimulationConstants.TelemetryData.WHEELS_UP,
                SimulationConstants.TelemetryData.WHEELS_DOWN,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateSpeedItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.SPEED_MAX_VALUE),
                SimulationConstants.Units.KILOMETERS_PER_HOUR,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.SPEED_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateYawItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ANGLE_FULL_ROTATION),
                SimulationConstants.Units.DEGREES,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.ANGLE_FULL_ROTATION,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateOffsetAngleItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_NEGATIVE, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_POSITIVE),
                SimulationConstants.Units.DEGREES,
                SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_NEGATIVE,
                SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_POSITIVE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateThrustItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.THRUST_MAX_VALUE),
                SimulationConstants.Units.NEWTONS,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.THRUST_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateDataStorageItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.DATA_STORAGE_MAX_VALUE),
                SimulationConstants.Units.GIGABYTES,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.DATA_STORAGE_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateTimeItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.FLIGHT_TIME_MAX_VALUE),
                SimulationConstants.Units.SECONDS,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.FLIGHT_TIME_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateSignalStrengthItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MIN, SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MAX),
                SimulationConstants.Units.DECIBEL_MILLIWATTS,
                SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MIN,
                SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MAX,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateRpmItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.RPM_MAX_VALUE),
                SimulationConstants.Units.REVOLUTIONS_PER_MINUTE,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.RPM_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateEngineDegreesItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ENGINE_TEMPERATURE_MAX),
                SimulationConstants.Units.CELSIUS,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.ENGINE_TEMPERATURE_MAX,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateSleeveIdItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.SLEEVE_ID_MAX_VALUE),
                SimulationConstants.Units.ID,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.SLEEVE_ID_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private ICDItem CreateChecksumItem(TelemetryFields field, int bitOffset, int bitLength)
        {
            return new ICDItem(
                field,
                typeof(double),
                GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.CHECKSUM_MAX_VALUE),
                SimulationConstants.Units.CHECKSUM,
                SimulationConstants.ICDGeneration.DEFAULT_VALUE,
                SimulationConstants.ICDGeneration.CHECKSUM_MAX_VALUE,
                bitOffset,
                bitLength
            );
        }

        private static double GenerateRandomValue(double minValue, double maxValue)
        {
            if (Math.Abs(maxValue - minValue) < SimulationConstants.TelemetryCompression.EPSILON_COMPARISON)
                return minValue;

            return _random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
} 