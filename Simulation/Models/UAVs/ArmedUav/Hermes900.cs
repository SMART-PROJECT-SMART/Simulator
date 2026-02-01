using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Channels;

namespace Simulation.Models.UAVs.ArmedUav
{
    public class Hermes900 : ArmedUAV
    {
        public Hermes900(int tailId, Location startLocation)
            : this(startLocation, tailId, BuildProperties(), new List<Channel>()) { }

        public Hermes900(int tailId, Location startLocation, List<Channel> channels)
            : this(startLocation, tailId, BuildProperties(), channels) { }

        public Hermes900(
            Location startLocation,
            int tailId,
            Dictionary<UAVProperties, double> properties,
            List<Channel> channels
        )
            : base(
                startLocation,
                tailId,
                SimulationConstants.Hermes900_Constants.FuelTankCapacity,
                properties,
                channels
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
            TelemetryData[TelemetryFields.PlatformType] = (double)PlatformType.Hermes900;

            WeaponAmmo[WeaponType.Hellfire] = (int)
                SimulationConstants.Hermes900_Constants.HellfireAmmo;
            WeaponAmmo[WeaponType.SpikeNLOS] = (int)
                SimulationConstants.Hermes900_Constants.SpikeNLOSAmmo;
            WeaponAmmo[WeaponType.Griffin] = (int)
                SimulationConstants.Hermes900_Constants.GriffinAmmo;
        }

        private static Dictionary<UAVProperties, double> BuildProperties()
        {
            return new Dictionary<UAVProperties, double>
            {
                [UAVProperties.Mass] = SimulationConstants.Hermes900_Constants.Mass,
                [UAVProperties.FrontalSurface] = SimulationConstants
                    .Hermes900_Constants
                    .FrontalSurface,
                [UAVProperties.WingsSurface] = SimulationConstants.Hermes900_Constants.WingsSurface,
                [UAVProperties.ThrustMax] = SimulationConstants.Hermes900_Constants.ThrustMax,
                [UAVProperties.MaxCruiseSpeed] = SimulationConstants
                    .Hermes900_Constants
                    .MaxCruiseSpeedKmph,
                [UAVProperties.MaxAcceleration] = SimulationConstants
                    .Hermes900_Constants
                    .MaxAcceleration,
                [UAVProperties.MaxDeceleration] = SimulationConstants
                    .Hermes900_Constants
                    .MaxDeceleration,
                [UAVProperties.FuelTankCapacity] = SimulationConstants
                    .Hermes900_Constants
                    .FuelTankCapacity,
                [UAVProperties.FuelConsumption] = SimulationConstants
                    .Hermes900_Constants
                    .FuelConsumption,
                [UAVProperties.TransmitPower] = SimulationConstants
                    .Hermes900_Constants
                    .TransmitPowerDbm,
                [UAVProperties.TransmitAntennaGain] = SimulationConstants
                    .Hermes900_Constants
                    .TransmitAntennaGainDbi,
                [UAVProperties.ReceiveAntennaGain] = SimulationConstants
                    .Hermes900_Constants
                    .ReceiveAntennaGainDbi,
                [UAVProperties.TransmitLoss] = SimulationConstants
                    .Hermes900_Constants
                    .TransmitLossDb,
                [UAVProperties.ReceiveLoss] = SimulationConstants.Hermes900_Constants.ReceiveLossDb,
                [UAVProperties.Frequency] = SimulationConstants.Hermes900_Constants.FrequencyHz,
                [UAVProperties.PropellerRadius] = SimulationConstants
                    .Hermes900_Constants
                    .PropellerRadius,
            };
        }
    }
}
