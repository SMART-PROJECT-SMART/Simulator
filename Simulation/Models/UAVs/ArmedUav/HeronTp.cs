using Simulation.Common.Enums;
using Simulation.Common.constants;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class HeronTp : ArmedUAV
    {
        private static Dictionary<WeaponType, int> DefaultWeaponsAmmo => new()
        {
            { WeaponType.Hellfire, 4 },
            { WeaponType.Griffin, 2 },
            { WeaponType.JDAM, 1 }
        };

        public HeronTp(
            int tailId,
            Location startLocation,
            bool isWeaponSystemArmed = false,
            double weaponSystemStatus = SimulationConstants.UAV_Constants.ONE_HUNDRED_PRECENT
        ) : base(
            startLocation,
            SimulationConstants.HeronTP_Constants.FuelTankSize,
            SimulationConstants.HeronTP_Constants.MaxCruiseSpeedKmph,
            SimulationConstants.HeronTP_Constants.MaxAcceleration,
            SimulationConstants.HeronTP_Constants.MaxDeceleration,
            tailId,
            DefaultWeaponsAmmo,
            isWeaponSystemArmed,
            weaponSystemStatus
        )
        {
        }
    }
}
