using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.ICD;
using Newtonsoft.Json;

namespace Simulation.Services.ICD
{
    public class ICDGenerator
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
            { TelemetryFields.Checksum, SimulationConstants.TelemetryCompression.CHECKSUM_BITS },
        };

        public static string GenerateICD(Dictionary<TelemetryFields, double> telemetryData)
        {
            var generator = new ICDGenerator();
            generator.GenerateTwoICDDocuments().Wait();
            return Path.Combine(SimulationConstants.ICDGeneration.ICD_DIRECTORY, "north_telemetry_icd.json");
        }

        public async Task GenerateTwoICDDocuments()
        {
            var northTelemetryItems = GenerateNorthTelemetryICDItems();
            var southTelemetryItems = GenerateSouthTelemetryICDItems();
            
            Directory.CreateDirectory(SimulationConstants.ICDGeneration.ICD_DIRECTORY);
            
            await SaveNorthJsonAsync(northTelemetryItems);
            await SaveSouthJsonAsync(southTelemetryItems);
        }

        private List<ICDItem> GenerateNorthTelemetryICDItems()
        {
            var items = new List<ICDItem>();
            int currentBitOffset = (int)SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE;

            var northFields = new[]
            {
                TelemetryFields.DragCoefficient,
                TelemetryFields.LiftCoefficient,
                TelemetryFields.ThrottlePercent,
                TelemetryFields.CruiseAltitude,
                TelemetryFields.Latitude,
                TelemetryFields.LandingGearStatus,
                TelemetryFields.Longitude,
                TelemetryFields.Altitude,
                TelemetryFields.CurrentSpeedKmph,
                TelemetryFields.YawDeg,
                TelemetryFields.PitchDeg,
                TelemetryFields.RollDeg,
                TelemetryFields.ThrustAfterInfluence,
                TelemetryFields.FuelAmount,
                TelemetryFields.DataStorageUsedGB,
                TelemetryFields.FlightTimeSec,
                TelemetryFields.SignalStrength,
                TelemetryFields.Rpm,
                TelemetryFields.EngineDegrees,
                TelemetryFields.NearestSleeveId
            };

            foreach (var field in northFields)
            {
                var icdItem = CreateTelemetryICDItem(field, currentBitOffset);
                items.Add(icdItem);
                currentBitOffset += _sizeInBits[field];
            }

            var checksumItem = CreateTelemetryICDItem(TelemetryFields.Checksum, currentBitOffset);
            items.Add(checksumItem);

            return items;
        }

        private List<ICDItem> GenerateSouthTelemetryICDItems()
        {
            var items = new List<ICDItem>();
            int currentBitOffset = (int)SimulationConstants.TelemetryCompression.CLAMP_MIN_VALUE;

            var southFields = new[]
            {
                TelemetryFields.DragCoefficient,
                TelemetryFields.LiftCoefficient,
                TelemetryFields.ThrottlePercent,
                TelemetryFields.CruiseAltitude,
                TelemetryFields.Latitude,
                TelemetryFields.LandingGearStatus,
                TelemetryFields.Longitude,
                TelemetryFields.Altitude,
                TelemetryFields.CurrentSpeedKmph,
                TelemetryFields.YawDeg,
                TelemetryFields.PitchDeg,
                TelemetryFields.RollDeg,
                TelemetryFields.ThrustAfterInfluence,
                TelemetryFields.FuelAmount,
                TelemetryFields.DataStorageUsedGB,
                TelemetryFields.FlightTimeSec,
                TelemetryFields.SignalStrength,
                TelemetryFields.Rpm,
                TelemetryFields.EngineDegrees,
                TelemetryFields.NearestSleeveId
            };

            foreach (var field in southFields)
            {
                var icdItem = CreateTelemetryICDItem(field, currentBitOffset);
                items.Add(icdItem);
                currentBitOffset += _sizeInBits[field];
            }

            var checksumItem = CreateTelemetryICDItem(TelemetryFields.Checksum, currentBitOffset);
            items.Add(checksumItem);

            return items;
        }

        private double GenerateRandomValue(double minValue, double maxValue)
        {
            if (Math.Abs(maxValue - minValue) < 0.0001)
                return minValue;
            
            return _random.NextDouble() * (maxValue - minValue) + minValue;
        }

        private ICDItem CreateTelemetryICDItem(TelemetryFields field, int bitOffset)
        {
            return field switch
            {
                TelemetryFields.DragCoefficient => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.COEFFICIENT_MAX_VALUE), SimulationConstants.Units.COEFFICIENT, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.COEFFICIENT_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.LiftCoefficient => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.COEFFICIENT_MAX_VALUE), SimulationConstants.Units.COEFFICIENT, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.COEFFICIENT_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.ThrottlePercent => new ICDItem(field, typeof(byte), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.PERCENTAGE_MAX_VALUE), SimulationConstants.Units.PERCENTAGE, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.PERCENTAGE_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.CruiseAltitude => new ICDItem(field, typeof(ushort), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ALTITUDE_MAX_VALUE), SimulationConstants.Units.METERS, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ALTITUDE_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.Latitude => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MIN, SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MAX), SimulationConstants.Units.DEGREES, SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MIN, SimulationConstants.ICDGeneration.COORDINATE_LATITUDE_MAX, bitOffset, _sizeInBits[field]),
                TelemetryFields.LandingGearStatus => new ICDItem(field, typeof(bool), GenerateRandomValue(SimulationConstants.TelemetryData.WHEELS_UP, SimulationConstants.TelemetryData.WHEELS_DOWN), SimulationConstants.Units.BOOLEAN, SimulationConstants.TelemetryData.WHEELS_UP, SimulationConstants.TelemetryData.WHEELS_DOWN, bitOffset, _sizeInBits[field]),
                TelemetryFields.Longitude => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MIN, SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MAX), SimulationConstants.Units.DEGREES, SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MIN, SimulationConstants.ICDGeneration.COORDINATE_LONGITUDE_MAX, bitOffset, _sizeInBits[field]),
                TelemetryFields.Altitude => new ICDItem(field, typeof(ushort), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ALTITUDE_MAX_VALUE), SimulationConstants.Units.METERS, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ALTITUDE_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.CurrentSpeedKmph => new ICDItem(field, typeof(ushort), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.SPEED_MAX_VALUE), SimulationConstants.Units.KILOMETERS_PER_HOUR, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.SPEED_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.YawDeg => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ANGLE_FULL_ROTATION), SimulationConstants.Units.DEGREES, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ANGLE_FULL_ROTATION, bitOffset, _sizeInBits[field]),
                TelemetryFields.PitchDeg => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_NEGATIVE, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_POSITIVE), SimulationConstants.Units.DEGREES, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_NEGATIVE, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_POSITIVE, bitOffset, _sizeInBits[field]),
                TelemetryFields.RollDeg => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_NEGATIVE, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_POSITIVE), SimulationConstants.Units.DEGREES, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_NEGATIVE, SimulationConstants.ICDGeneration.ANGLE_HALF_ROTATION_POSITIVE, bitOffset, _sizeInBits[field]),
                TelemetryFields.ThrustAfterInfluence => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.THRUST_MAX_VALUE), SimulationConstants.Units.NEWTONS, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.THRUST_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.FuelAmount => new ICDItem(field, typeof(byte), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.PERCENTAGE_MAX_VALUE), SimulationConstants.Units.PERCENTAGE, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.PERCENTAGE_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.DataStorageUsedGB => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.DATA_STORAGE_MAX_VALUE), SimulationConstants.Units.GIGABYTES, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.DATA_STORAGE_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.FlightTimeSec => new ICDItem(field, typeof(uint), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.FLIGHT_TIME_MAX_VALUE), SimulationConstants.Units.SECONDS, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.FLIGHT_TIME_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.SignalStrength => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MIN, SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MAX), SimulationConstants.Units.DECIBEL_MILLIWATTS, SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MIN, SimulationConstants.ICDGeneration.SIGNAL_STRENGTH_MAX, bitOffset, _sizeInBits[field]),
                TelemetryFields.Rpm => new ICDItem(field, typeof(ushort), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.RPM_MAX_VALUE), SimulationConstants.Units.REVOLUTIONS_PER_MINUTE, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.RPM_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.EngineDegrees => new ICDItem(field, typeof(double), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ENGINE_TEMPERATURE_MAX), SimulationConstants.Units.CELSIUS, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.ENGINE_TEMPERATURE_MAX, bitOffset, _sizeInBits[field]),
                TelemetryFields.NearestSleeveId => new ICDItem(field, typeof(ushort), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.SLEEVE_ID_MAX_VALUE), SimulationConstants.Units.ID, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.SLEEVE_ID_MAX_VALUE, bitOffset, _sizeInBits[field]),
                TelemetryFields.Checksum => new ICDItem(field, typeof(uint), GenerateRandomValue(SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.CHECKSUM_MAX_VALUE), SimulationConstants.Units.CHECKSUM, SimulationConstants.ICDGeneration.DEFAULT_VALUE, SimulationConstants.ICDGeneration.CHECKSUM_MAX_VALUE, bitOffset, _sizeInBits[field]),
                _ => throw new ArgumentException($"Unknown telemetry field: {field}")
            };
        }

        private async Task SaveNorthJsonAsync(List<ICDItem> icdItems)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            var icdDocument = new
            {
                TelemetryFields = icdItems
            };

            string jsonContent = JsonConvert.SerializeObject(icdDocument, settings);
            string jsonFilePath = Path.Combine(SimulationConstants.ICDGeneration.ICD_DIRECTORY, "north_telemetry_icd.json");
            
            await File.WriteAllTextAsync(jsonFilePath, jsonContent);
        }

        private async Task SaveSouthJsonAsync(List<ICDItem> icdItems)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            var icdDocument = new
            {
                TelemetryFields = icdItems
            };

            string jsonContent = JsonConvert.SerializeObject(icdDocument, settings);
            string jsonFilePath = Path.Combine(SimulationConstants.ICDGeneration.ICD_DIRECTORY, "south_telemetry_icd.json");
            
            await File.WriteAllTextAsync(jsonFilePath, jsonContent);
        }
    }
}
