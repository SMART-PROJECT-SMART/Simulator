using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class Hermes900 : ArmedUAV
    {
        public Hermes900(int tailId, Location startLocation)
            : base(
                startLocation,
                tailId,
                SimulationConstants.Hermes900_Constants.FuelTankCapacity,
                SimulationConstants.Hermes900_Constants.SpecificFuelConsumption
            )
        {
            TelemetryData[TelemetryFields.Mass] = SimulationConstants.Hermes900_Constants.Mass;
            TelemetryData[TelemetryFields.FrontalSurface] = SimulationConstants
                .Hermes900_Constants
                .FrontalSurface;
            TelemetryData[TelemetryFields.WingsSurface] = SimulationConstants
                .Hermes900_Constants
                .WingsSurface;
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Hermes900_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Hermes900_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.ThrustMax] = SimulationConstants
                .Hermes900_Constants
                .ThrustMax;
            TelemetryData[TelemetryFields.MaxCruiseSpeedKmph] = SimulationConstants
                .Hermes900_Constants
                .MaxCruiseSpeedKmph;
            TelemetryData[TelemetryFields.MaxAccelerationMps2] = SimulationConstants
                .Hermes900_Constants
                .MaxAcceleration;
            TelemetryData[TelemetryFields.MaxDecelerationMps2] = SimulationConstants
                .Hermes900_Constants
                .MaxDeceleration;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Hermes900_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Hermes900_Constants
                .ThrustAfterInfluence;
            TelemetryData[TelemetryFields.HellfireAmmo] = SimulationConstants
                .Hermes900_Constants
                .HellfireAmmo;
            TelemetryData[TelemetryFields.SpikeNLOSAmmo] = SimulationConstants
                .Hermes900_Constants
                .SpikeNLOSAmmo;
            TelemetryData[TelemetryFields.GriffinAmmo] = SimulationConstants
                .Hermes900_Constants
                .GriffinAmmo;
        }
    }
}
