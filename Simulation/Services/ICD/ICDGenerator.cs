using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.ICD.Factories;
using Simulation.Services.ICD.Interfaces;
using Simulation.Services.ICD.Strategies;

namespace Simulation.Services.ICD
{
    public class ICDGenerator
    {
        private readonly IICDGenerationStrategy _northStrategy;
        private readonly IICDGenerationStrategy _southStrategy;

        public ICDGenerator()
        {
            var itemFactory = new TelemetryICDItemFactory();
            _northStrategy = new TelemetryICDGenerationStrategy(itemFactory);
            _southStrategy = new TelemetryICDGenerationStrategy(itemFactory);
        }

        public static string GenerateICD(Dictionary<TelemetryFields, double> telemetryData)
        {
            var generator = new ICDGenerator();
            generator.GenerateTwoICDDocuments().Wait();
            return Path.Combine(
                SimulationConstants.ICDGeneration.ICD_DIRECTORY,
                "north_telemetry_icd.json"
            );
        }

        public async Task GenerateTwoICDDocuments()
        {
            var northTelemetryItems = _northStrategy.GenerateICDItems();
            var southTelemetryItems = _southStrategy.GenerateICDItems();

            await _northStrategy.SaveICDAsync(northTelemetryItems, "north_telemetry_icd.json");
            await _southStrategy.SaveICDAsync(southTelemetryItems, "south_telemetry_icd.json");
        }
    }
}
