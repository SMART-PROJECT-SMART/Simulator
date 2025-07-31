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
                SimulationConstants.Hermes900_Constants.Mass,
                SimulationConstants.Hermes900_Constants.FrontalSurface,
                SimulationConstants.Hermes900_Constants.WingsSurface,
                SimulationConstants.Hermes900_Constants.ThrustMax,
                SimulationConstants.Hermes900_Constants.MaxCruiseSpeedKmph,
                SimulationConstants.Hermes900_Constants.MaxAcceleration,
                SimulationConstants.Hermes900_Constants.MaxDeceleration,
                SimulationConstants.Hermes900_Constants.FuelTankCapacity,
                SimulationConstants.Hermes900_Constants.FuelConsumption
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Hermes900_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Hermes900_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Hermes900_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Hermes900_Constants
                .ThrustAfterInfluence;

            WeaponAmmo[WeaponType.Hellfire] = (int)
                SimulationConstants.Hermes900_Constants.HellfireAmmo;
            WeaponAmmo[WeaponType.SpikeNLOS] = (int)
                SimulationConstants.Hermes900_Constants.SpikeNLOSAmmo;
            WeaponAmmo[WeaponType.Griffin] = (int)
                SimulationConstants.Hermes900_Constants.GriffinAmmo;
        }
    }
}
