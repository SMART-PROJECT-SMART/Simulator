using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Searcher : SurveillanceUAV
    {
        public Searcher(int tailId, Location startLocation)
            : base(
                startLocation,
                tailId,
                SimulationConstants.Searcher_Constants.Mass,
                SimulationConstants.Searcher_Constants.FrontalSurface,
                SimulationConstants.Searcher_Constants.WingsSurface,
                SimulationConstants.Searcher_Constants.ThrustMax,
                SimulationConstants.Searcher_Constants.MaxCruiseSpeedKmph,
                SimulationConstants.Searcher_Constants.MaxAcceleration,
                SimulationConstants.Searcher_Constants.MaxDeceleration,
                SimulationConstants.Searcher_Constants.FuelTankCapacity,
                SimulationConstants.Searcher_Constants.FuelConsumption,
                SimulationConstants.Searcher_Constants.DataStorageCapacityGB
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Searcher_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Searcher_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Searcher_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Searcher_Constants
                .ThrustAfterInfluence;

            SensorStatus[SensorType.ElectroOptical] = true;
            SensorStatus[SensorType.InfraredImaging] = true;
            SensorStatus[SensorType.WeatherRadar] = true;
        }
    }
}
