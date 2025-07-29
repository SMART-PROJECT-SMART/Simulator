using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        protected Dictionary<WeaponType, int> WeaponsAmmo { get;  set; }
        protected bool IsWeaponSystemArmed { get; set; } = false;
        protected double WeaponSystemStatus { get; set; }

        protected ArmedUAV(
            Location startLocation,
            double fuelTankSize,
            double maxCruiseSpeed,
            double maxAcceleration,
            double maxDeceleration,
            int tailId,
            Dictionary<WeaponType, int> weaponsAmmo,
            bool isWeaponSystemArmed = false,
            double weaponSystemStatus = SimulationConstants.UAV_Constants.ONE_HUNDRED_PRECENT
        ) : base(startLocation, tailId)
        {
            WeaponsAmmo = weaponsAmmo;
            IsWeaponSystemArmed = isWeaponSystemArmed;
            WeaponSystemStatus = weaponSystemStatus;
        }
    }
}
