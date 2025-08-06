using System.Collections.Generic;
using Simulation.Common.Enums;
using Simulation.Models.Channels;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        public Dictionary<WeaponType, int> WeaponAmmo { get; set; }

        protected ArmedUAV(
            Location startLocation,
            int tailId,
            double fuelAmount,
            Dictionary<UAVProperties, double> properties,
            List<Channel> channels)
            : base(startLocation, tailId, fuelAmount, properties,channels)
        {
            WeaponAmmo = new Dictionary<WeaponType, int>();
            properties[UAVProperties.IsWeaponSystemArmed] = 0.0;
            properties[UAVProperties.WeaponSystemHealth] = 100.0;
        }
    }
}
