using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class HeronTp : ArmedUAV
    {
        public HeronTp(int tailId, Location startLocation)
            : base(startLocation, tailId,
                SimulationConstants.HeronTP_Constants.FuelTankCapacity,SimulationConstants.HeronTP_Constants.FuelConsumption)
        {
            TelemetryData[TelemetryFields.Mass] = SimulationConstants.HeronTP_Constants.Mass;
            TelemetryData[TelemetryFields.FrontalSurface] = SimulationConstants
                .HeronTP_Constants
                .FrontalSurface;
            TelemetryData[TelemetryFields.WingsSurface] = SimulationConstants
                .HeronTP_Constants
                .WingsSurface;
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .HeronTP_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .HeronTP_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.ThrustMax] = SimulationConstants
                .HeronTP_Constants
                .ThrustMax;
            TelemetryData[TelemetryFields.MaxCruiseSpeedKmph] = SimulationConstants
                .HeronTP_Constants
                .MaxCruiseSpeedKmph;
            TelemetryData[TelemetryFields.MaxAccelerationMps2] = SimulationConstants
                .HeronTP_Constants
                .MaxAcceleration;
            TelemetryData[TelemetryFields.MaxDecelerationMps2] = SimulationConstants
                .HeronTP_Constants
                .MaxDeceleration;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .HeronTP_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .HeronTP_Constants
                .ThrustAfterInfluence;
            TelemetryData[TelemetryFields.HellfireAmmo] = SimulationConstants
                .HeronTP_Constants
                .HellfireAmmo;
            TelemetryData[TelemetryFields.GriffinAmmo] = SimulationConstants
                .HeronTP_Constants
                .GriffinAmmo;
            TelemetryData[TelemetryFields.JDAMAmmo] = SimulationConstants
                .HeronTP_Constants
                .JDAMAmmo;
        }
    }
}
