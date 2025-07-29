using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class Hermes900 : ArmedUAV
    {
        private static Dictionary<WeaponType, int> DefaultWeaponsAmmo => new()
        {
            { WeaponType.Hellfire, 2 },
            { WeaponType.SpikeNLOS, 2 },
            { WeaponType.Griffin, 1 }
        };

        public Hermes900(
            int tailId,
            Location startLocation,
            bool isWeaponSystemArmed = false,
            double weaponSystemStatus = SimulationConstants.UAV_Constants.ONE_HUNDRED_PRECENT
        ) : base(
            startLocation,
            SimulationConstants.Hermes900_Constants.FuelTankSize,
            SimulationConstants.Hermes900_Constants.MaxCruiseSpeedKmph,
            SimulationConstants.Hermes900_Constants.MaxAcceleration,
            SimulationConstants.Hermes900_Constants.MaxDeceleration,
            tailId,
            DefaultWeaponsAmmo,
            isWeaponSystemArmed,
            weaponSystemStatus
        )
        {
        }
    }
}
