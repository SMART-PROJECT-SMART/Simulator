using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Hermes450 : SurveillanceUAV
    {
        public Hermes450(int tailId, Location startLocation)
            : base(startLocation, tailId,
                SimulationConstants.Hermes450_Constants.FuelTankCapacity,SimulationConstants.Hermes450_Constants.FuelConsumption)
        {
            TelemetryData[TelemetryFields.Mass] = SimulationConstants.Hermes450_Constants.Mass;
            TelemetryData[TelemetryFields.FrontalSurface] = SimulationConstants
                .Hermes450_Constants
                .FrontalSurface;
            TelemetryData[TelemetryFields.WingsSurface] = SimulationConstants
                .Hermes450_Constants
                .WingsSurface;
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Hermes450_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Hermes450_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.ThrustMax] = SimulationConstants
                .Hermes450_Constants
                .ThrustMax;
            TelemetryData[TelemetryFields.MaxCruiseSpeedKmph] = SimulationConstants
                .Hermes450_Constants
                .MaxCruiseSpeedKmph;
            TelemetryData[TelemetryFields.MaxAccelerationMps2] = SimulationConstants
                .Hermes450_Constants
                .MaxAcceleration;
            TelemetryData[TelemetryFields.MaxDecelerationMps2] = SimulationConstants
                .Hermes450_Constants
                .MaxDeceleration;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Hermes450_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Hermes450_Constants
                .ThrustAfterInfluence;
            TelemetryData[TelemetryFields.DataStorageCapacityGB] = SimulationConstants
                .Hermes450_Constants
                .DataStorageCapacityGB;
            TelemetryData[TelemetryFields.ElectroOpticalSensorStatus] = 1.0;
            TelemetryData[TelemetryFields.InfraredImagingSensorStatus] = 1.0;
            TelemetryData[TelemetryFields.WeatherRadarStatus] = 1.0;
        }
    }
}
