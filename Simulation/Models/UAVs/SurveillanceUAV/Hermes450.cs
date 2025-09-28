using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Channels;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Hermes450 : SurveillanceUAV
    {
        public Hermes450(int tailId, Location startLocation)
            : this(
                startLocation,
                tailId,
                BuildProperties(),
                SimulationConstants.Hermes450_Constants.DataStorageCapacityGB,
                new List<Channel>()
            ) { }

        public Hermes450(int tailId, Location startLocation, List<Channel> channels)
            : this(
                startLocation,
                tailId,
                BuildProperties(),
                SimulationConstants.Hermes450_Constants.DataStorageCapacityGB,
                channels
            ) { }

        public Hermes450(
            Location startLocation,
            int tailId,
            Dictionary<UAVProperties, double> properties,
            double dataStorageCapacityGB,
            List<Channel> channels
        )
            : base(
                startLocation,
                tailId,
                SimulationConstants.Hermes450_Constants.FuelTankCapacity,
                properties,
                dataStorageCapacityGB,
                channels
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Hermes450_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Hermes450_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Hermes450_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Hermes450_Constants
                .ThrustAfterInfluence;

            SensorStatus[SensorType.ElectroOptical] = true;
            SensorStatus[SensorType.InfraredImaging] = true;
            SensorStatus[SensorType.WeatherRadar] = true;
        }

        private static Dictionary<UAVProperties, double> BuildProperties()
        {
            return new Dictionary<UAVProperties, double>
            {
                [UAVProperties.Mass] = SimulationConstants.Hermes450_Constants.Mass,
                [UAVProperties.FrontalSurface] = SimulationConstants
                    .Hermes450_Constants
                    .FrontalSurface,
                [UAVProperties.WingsSurface] = SimulationConstants.Hermes450_Constants.WingsSurface,
                [UAVProperties.ThrustMax] = SimulationConstants.Hermes450_Constants.ThrustMax,
                [UAVProperties.MaxCruiseSpeed] = SimulationConstants
                    .Hermes450_Constants
                    .MaxCruiseSpeedKmph,
                [UAVProperties.MaxAcceleration] = SimulationConstants
                    .Hermes450_Constants
                    .MaxAcceleration,
                [UAVProperties.MaxDeceleration] = SimulationConstants
                    .Hermes450_Constants
                    .MaxDeceleration,
                [UAVProperties.FuelTankCapacity] = SimulationConstants
                    .Hermes450_Constants
                    .FuelTankCapacity,
                [UAVProperties.FuelConsumption] = SimulationConstants
                    .Hermes450_Constants
                    .FuelConsumption,
                [UAVProperties.TransmitPower] = SimulationConstants
                    .Hermes450_Constants
                    .TransmitPowerDbm,
                [UAVProperties.TransmitAntennaGain] = SimulationConstants
                    .Hermes450_Constants
                    .TransmitAntennaGainDbi,
                [UAVProperties.ReceiveAntennaGain] = SimulationConstants
                    .Hermes450_Constants
                    .ReceiveAntennaGainDbi,
                [UAVProperties.TransmitLoss] = SimulationConstants
                    .Hermes450_Constants
                    .TransmitLossDb,
                [UAVProperties.ReceiveLoss] = SimulationConstants.Hermes450_Constants.ReceiveLossDb,
                [UAVProperties.Frequency] = SimulationConstants.Hermes450_Constants.FrequencyHz,
                [UAVProperties.PropellerRadius] = SimulationConstants
                    .Hermes450_Constants
                    .PropellerRadius,
            };
        }
    }
}
