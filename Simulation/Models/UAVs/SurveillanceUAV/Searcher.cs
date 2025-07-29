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
            double dataStorageCapacityGb = SimulationConstants.Searcher_Constants.DataStorageCapacityGB
        ) : base(
            startLocation,
            SimulationConstants.Searcher_Constants.FuelTankSize,
            SimulationConstants.Searcher_Constants.MaxCruiseSpeedKmph,
            SimulationConstants.Searcher_Constants.MaxAcceleration,
            SimulationConstants.Searcher_Constants.MaxDeceleration,
            tailId,
            DefaultSensorsStatus,
            dataStorageCapacityGb
        )
        {
            CurrentMissionId = string.Empty;
        }
    }
}
