using System.Collections.Generic;
using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class HeronTp : ArmedUAV
    {
        public HeronTp(int tailId, Location startLocation)
            : this(startLocation, tailId, BuildProperties()) { }

        public HeronTp(
            Location startLocation,
            int tailId,
            Dictionary<UAVProperties, double> properties
        )
            : base(
                startLocation,
                tailId,
                SimulationConstants.HeronTP_Constants.FuelTankCapacity,
                properties
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .HeronTP_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .HeronTP_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .HeronTP_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .HeronTP_Constants
                .ThrustAfterInfluence;

            WeaponAmmo[WeaponType.Hellfire] = (int)
                SimulationConstants.HeronTP_Constants.HellfireAmmo;
            WeaponAmmo[WeaponType.Griffin] = (int)SimulationConstants.HeronTP_Constants.GriffinAmmo;
            WeaponAmmo[WeaponType.JDAM] = (int)SimulationConstants.HeronTP_Constants.JDAMAmmo;
        }

        private static Dictionary<UAVProperties, double> BuildProperties()
        {
            return new Dictionary<UAVProperties, double>
            {
                [UAVProperties.Mass] = SimulationConstants.HeronTP_Constants.Mass,
                [UAVProperties.FrontalSurface] = SimulationConstants
                    .HeronTP_Constants
                    .FrontalSurface,
                [UAVProperties.WingsSurface] = SimulationConstants.HeronTP_Constants.WingsSurface,
                [UAVProperties.ThrustMax] = SimulationConstants.HeronTP_Constants.ThrustMax,
                [UAVProperties.MaxCruiseSpeed] = SimulationConstants
                    .HeronTP_Constants
                    .MaxCruiseSpeedKmph,
                [UAVProperties.MaxAcceleration] = SimulationConstants
                    .HeronTP_Constants
                    .MaxAcceleration,
                [UAVProperties.MaxDeceleration] = SimulationConstants
                    .HeronTP_Constants
                    .MaxDeceleration,
                [UAVProperties.FuelTankCapacity] = SimulationConstants
                    .HeronTP_Constants
                    .FuelTankCapacity,
                [UAVProperties.FuelConsumption] = SimulationConstants
                    .HeronTP_Constants
                    .FuelConsumption,

                [UAVProperties.TransmitPower] = SimulationConstants
                    .HeronTP_Constants
                    .TransmitPowerDbm,
                [UAVProperties.TransmitAntennaGain] = SimulationConstants
                    .HeronTP_Constants
                    .TransmitAntennaGainDbi,
                [UAVProperties.ReceiveAntennaGain] = SimulationConstants
                    .HeronTP_Constants
                    .ReceiveAntennaGainDbi,
                [UAVProperties.TransmitLoss] = SimulationConstants.HeronTP_Constants.TransmitLossDb,
                [UAVProperties.ReceiveLoss] = SimulationConstants.HeronTP_Constants.ReceiveLossDb,
                [UAVProperties.Frequency] = SimulationConstants.HeronTP_Constants.FrequencyHz,
            };
        }
    }
}
