using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Searcher : SurveillanceUAV
    {
        public Searcher(int tailId, Location startLocation)
            : base(startLocation, tailId,
                SimulationConstants.Searcher_Constants.FuelTankCapacity,SimulationConstants.Searcher_Constants.FuelConsumption)
        {
            TelemetryData[TelemetryFields.Mass] = SimulationConstants.Searcher_Constants.Mass;
            TelemetryData[TelemetryFields.FrontalSurface] = SimulationConstants
                .Searcher_Constants
                .FrontalSurface;
            TelemetryData[TelemetryFields.WingsSurface] = SimulationConstants
                .Searcher_Constants
                .WingsSurface;
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Searcher_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Searcher_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.ThrustMax] = SimulationConstants
                .Searcher_Constants
                .ThrustMax;
            TelemetryData[TelemetryFields.MaxCruiseSpeedKmph] = SimulationConstants
                .Searcher_Constants
                .MaxCruiseSpeedKmph;
            TelemetryData[TelemetryFields.MaxAccelerationMps2] = SimulationConstants
                .Searcher_Constants
                .MaxAcceleration;
            TelemetryData[TelemetryFields.MaxDecelerationMps2] = SimulationConstants
                .Searcher_Constants
                .MaxDeceleration;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Searcher_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Searcher_Constants
                .ThrustAfterInfluence;
            TelemetryData[TelemetryFields.DataStorageCapacityGB] = SimulationConstants
                .Searcher_Constants
                .DataStorageCapacityGB;
            TelemetryData[TelemetryFields.ElectroOpticalSensorStatus] = 1.0;
            TelemetryData[TelemetryFields.InfraredImagingSensorStatus] = 1.0;
            TelemetryData[TelemetryFields.WeatherRadarStatus] = 1.0;
        }
    }
}
