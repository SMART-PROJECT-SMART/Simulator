using Newtonsoft.Json;
using Simulation.Common.Enums;
using Simulation.Common.constants;
using Simulation.Models.ICD;
using Simulation.Services.ICD.Interfaces;

namespace Simulation.Services.ICD.Strategies
{
    public class TelemetryICDGenerationStrategy : IICDGenerationStrategy
    {
        private readonly IICDItemFactory _itemFactory;
        private readonly TelemetryFields[] _dataFields;
        private readonly Dictionary<TelemetryFields, int> _sizeInBits;

        public TelemetryICDGenerationStrategy(IICDItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _dataFields = Enum.GetValues<TelemetryFields>()
                .Where(field => field != TelemetryFields.Checksum)
                .ToArray();
            
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
        }

        public List<ICDItem> GenerateICDItems()
        {
            var items = new List<ICDItem>();
            int currentBitOffset = 0;

            foreach (var field in _dataFields)
            {
                var icdItem = _itemFactory.CreateItem(field, currentBitOffset, _sizeInBits[field]);
                items.Add(icdItem);
                currentBitOffset += _sizeInBits[field];
            }

            var checksumItem = _itemFactory.CreateItem(TelemetryFields.Checksum, currentBitOffset, SimulationConstants.TelemetryCompression.CHECKSUM_BITS);
            items.Add(checksumItem);

            return items;
        }

        public async Task SaveICDAsync(List<ICDItem> icdItems, string filename)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            };

            var icdDocument = new { TelemetryFields = icdItems };
            string jsonContent = JsonConvert.SerializeObject(icdDocument, settings);
            
            Directory.CreateDirectory(SimulationConstants.ICDGeneration.ICD_DIRECTORY);
            string jsonFilePath = Path.Combine(SimulationConstants.ICDGeneration.ICD_DIRECTORY, filename);
            
            await File.WriteAllTextAsync(jsonFilePath, jsonContent);
        }
    }
} 