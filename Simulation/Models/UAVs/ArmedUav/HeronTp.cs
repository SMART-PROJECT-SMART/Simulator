using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class HeronTp : ArmedUAV
    {
        public HeronTp(int tailId, Location startLocation)
            : base(
                startLocation,
                tailId,
                SimulationConstants.HeronTP_Constants.Mass,
                SimulationConstants.HeronTP_Constants.FrontalSurface,
                SimulationConstants.HeronTP_Constants.WingsSurface,
                SimulationConstants.HeronTP_Constants.ThrustMax,
                SimulationConstants.HeronTP_Constants.MaxCruiseSpeedKmph,
                SimulationConstants.HeronTP_Constants.MaxAcceleration,
                SimulationConstants.HeronTP_Constants.MaxDeceleration,
                SimulationConstants.HeronTP_Constants.FuelTankCapacity,
                SimulationConstants.HeronTP_Constants.FuelConsumption
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants.HeronTP_Constants.DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants.HeronTP_Constants.LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants.HeronTP_Constants.CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants.HeronTP_Constants.ThrustAfterInfluence;

            WeaponAmmo[WeaponType.Hellfire] = (int)SimulationConstants.HeronTP_Constants.HellfireAmmo;
            WeaponAmmo[WeaponType.Griffin] = (int)SimulationConstants.HeronTP_Constants.GriffinAmmo;
            WeaponAmmo[WeaponType.JDAM] = (int)SimulationConstants.HeronTP_Constants.JDAMAmmo;
        }
    }
}
