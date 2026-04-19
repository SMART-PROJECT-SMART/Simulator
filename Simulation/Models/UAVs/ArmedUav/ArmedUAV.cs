using Core.Models;
﻿using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Channels;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        public Dictionary<WeaponType, int> WeaponAmmo { get; set; }
        public Dictionary<WeaponType, int> WeaponAmmoCapacity { get; set; }

        protected ArmedUAV(
            Location startLocation,
            int tailId,
            double fuelAmount,
            Dictionary<UAVProperties, double> properties,
            List<Channel> channels
        )
            : base(startLocation, tailId, fuelAmount, properties, channels)
        {
            TelemetryData[TelemetryFields.UAVTypeValue] = (double)UAVType.Armed;
            WeaponAmmo = new Dictionary<WeaponType, int>();
            WeaponAmmoCapacity = new Dictionary<WeaponType, int>();
            properties[UAVProperties.IsWeaponSystemArmed] = 0.0;
            properties[UAVProperties.WeaponSystemHealth] = 100.0;
            TelemetryData[TelemetryFields.AmmoPercentage] = SimulationConstants.TelemetryData.EMPTY_PERCENTAGE;
        }

        protected void SetWeaponAmmo(WeaponType weaponType, int ammoCount)
        {
            WeaponAmmo[weaponType] = ammoCount;
            if (!WeaponAmmoCapacity.ContainsKey(weaponType))
            {
                WeaponAmmoCapacity[weaponType] = ammoCount;
            }
            UpdateAmmoPercentageTelemetry();
        }

        protected void UpdateAmmoPercentageTelemetry()
        {
            int totalCurrentAmmo = WeaponAmmo.Values.Sum();
            int totalAmmoCapacity = WeaponAmmoCapacity.Values.Sum();
            if (totalAmmoCapacity <= 0)
            {
                TelemetryData[TelemetryFields.AmmoPercentage] = SimulationConstants
                    .TelemetryData
                    .EMPTY_PERCENTAGE;
                return;
            }
            TelemetryData[TelemetryFields.AmmoPercentage] = totalCurrentAmmo
                * SimulationConstants.TelemetryData.FULL_PERCENTAGE
                / totalAmmoCapacity;
        }
    }
}
