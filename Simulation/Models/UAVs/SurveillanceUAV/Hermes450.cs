using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Hermes450 : SurveillanceUAV
    {
        public Hermes450(int tailId, Location startLocation)
            : base(
                startLocation,
                tailId,
                SimulationConstants.Hermes450_Constants.Mass,
                SimulationConstants.Hermes450_Constants.FrontalSurface,
                SimulationConstants.Hermes450_Constants.WingsSurface,
                SimulationConstants.Hermes450_Constants.ThrustMax,
                SimulationConstants.Hermes450_Constants.MaxCruiseSpeedKmph,
                SimulationConstants.Hermes450_Constants.MaxAcceleration,
                SimulationConstants.Hermes450_Constants.MaxDeceleration,
                SimulationConstants.Hermes450_Constants.FuelTankCapacity,
                SimulationConstants.Hermes450_Constants.FuelConsumption,
                SimulationConstants.Hermes450_Constants.DataStorageCapacityGB
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Hermes450_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Hermes450_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Hermes450_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Hermes450_Constants
                .ThrustAfterInfluence;

            SensorStatus[SensorType.ElectroOptical] = true;
            SensorStatus[SensorType.InfraredImaging] = true;
            SensorStatus[SensorType.WeatherRadar] = true;
        }
    }
}
