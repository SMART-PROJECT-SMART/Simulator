using System.Collections.Generic;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        public Dictionary<WeaponType, int> WeaponAmmo { get; set; }

        protected ArmedUAV(
            Location startLocation,
            int tailId,
            double fuelAmount,
            Dictionary<UAVProperties,double> properties)
            : base(
                startLocation,
                tailId,
                fuelAmount,
                properties
            )
        {

            WeaponAmmo = new Dictionary<WeaponType, int>();
            properties[UAVProperties.IsWeaponSystemArmed] = 0.0;
            properties[UAVProperties.WeaponSystemHealth] = 100.0;
        }
    }
}
