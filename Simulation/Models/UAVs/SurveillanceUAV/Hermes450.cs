using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Hermes450 : SurveillanceUAV
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

        public Hermes450(
            int tailId,
            Location startLocation,
            double dataStorageCapacityGb = SimulationConstants.Hermes450_Constants.DataStorageCapacityGB
        ) : base(
            startLocation,
            SimulationConstants.Hermes450_Constants.FuelTankSize,
            SimulationConstants.Hermes450_Constants.MaxCruiseSpeedKmph,
            SimulationConstants.Hermes450_Constants.MaxAcceleration,
            SimulationConstants.Hermes450_Constants.MaxDeceleration,
            tailId,
          DefaultSensorsStatus,
            dataStorageCapacityGb
        )
        {
            CurrentMissionId = string.Empty;
        }
    }
}
