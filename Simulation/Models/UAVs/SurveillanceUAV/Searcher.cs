using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Searcher : SurveillanceUAV
    {
        private static Dictionary<SensorType, bool> DefaultSensorsStatus => new()
        {
            { SensorType.ElectroOptical, true },
            { SensorType.InfraredImaging, true },
            { SensorType.SyntheticApertureRadar, false },
            { SensorType.SIGINT, false },
            { SensorType.ELINT, false },
            { SensorType.WeatherRadar, true },
            { SensorType.LaserDesignator, false },
            { SensorType.MultiSpectralImaging, false },
            { SensorType.HyperspectralImaging, false },
            { SensorType.CommunicationsRelay, false }
        };

        public Searcher(
            int tailId,
            Location startLocation,
            Dictionary<TelemetryFields,double> initialTelemetry = null,
        double dataStorageCapacityGb = SimulationConstants.Searcher_Constants.DataStorageCapacityGB
        ) : base(
            startLocation,
            tailId,
            DefaultSensorsStatus,
            dataStorageCapacityGb
        )
        {
            TelemetryData = initialTelemetry;
            CurrentMissionId = string.Empty;
        }
    }
}
