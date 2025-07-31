using System.Collections.Generic;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        public Dictionary<WeaponType, int> WeaponAmmo { get; set; }
        public bool IsWeaponSystemArmed { get; set; }
        public double WeaponSystemHealth { get; set; }

        protected ArmedUAV(
            Location startLocation,
            int tailId,
            double mass,
            double frontalSurface,
            double wingsSurface,
            double thrustMax,
            double maxCruiseSpeed,
            double maxAccelerationMps2,
            double maxDecelerationMps2,
            double fuelTankCapacity,
            double fuelConsumption
        )
            : base(startLocation, tailId, mass, frontalSurface, wingsSurface, thrustMax, maxCruiseSpeed, maxAccelerationMps2, maxDecelerationMps2, fuelTankCapacity, fuelConsumption)
        {
            WeaponAmmo = new Dictionary<WeaponType, int>();
            IsWeaponSystemArmed = false; 
            WeaponSystemHealth = 100.0;
        }
    }
}