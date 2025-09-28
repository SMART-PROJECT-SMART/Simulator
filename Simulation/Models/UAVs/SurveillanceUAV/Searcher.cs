using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Channels;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public class Searcher : SurveillanceUAV
    {
        public Searcher(int tailId, Location startLocation)
            : this(
                startLocation,
                tailId,
                BuildProperties(),
                SimulationConstants.Searcher_Constants.DataStorageCapacityGB,
                new List<Channel>()
            ) { }

        public Searcher(int tailId, Location startLocation, List<Channel> channels)
            : this(
                startLocation,
                tailId,
                BuildProperties(),
                SimulationConstants.Searcher_Constants.DataStorageCapacityGB,
                channels
            ) { }

        public Searcher(
            Location startLocation,
            int tailId,
            Dictionary<UAVProperties, double> properties,
            double dataStorageCapacityGB,
            List<Channel> channels
        )
            : base(
                startLocation,
                tailId,
                SimulationConstants.Searcher_Constants.FuelTankCapacity,
                properties,
                dataStorageCapacityGB,
                channels
            )
        {
            TelemetryData[TelemetryFields.DragCoefficient] = SimulationConstants
                .Searcher_Constants
                .DragCoefficient;
            TelemetryData[TelemetryFields.LiftCoefficient] = SimulationConstants
                .Searcher_Constants
                .LiftCoefficient;
            TelemetryData[TelemetryFields.CruiseAltitude] = SimulationConstants
                .Searcher_Constants
                .CruiseAltitude;
            TelemetryData[TelemetryFields.ThrustAfterInfluence] = SimulationConstants
                .Searcher_Constants
                .ThrustAfterInfluence;

            SensorStatus[SensorType.ElectroOptical] = true;
            SensorStatus[SensorType.InfraredImaging] = true;
            SensorStatus[SensorType.WeatherRadar] = true;
        }

        private static Dictionary<UAVProperties, double> BuildProperties()
        {
            return new Dictionary<UAVProperties, double>
            {
                [UAVProperties.Mass] = SimulationConstants.Searcher_Constants.Mass,
                [UAVProperties.FrontalSurface] = SimulationConstants
                    .Searcher_Constants
                    .FrontalSurface,
                [UAVProperties.WingsSurface] = SimulationConstants.Searcher_Constants.WingsSurface,
                [UAVProperties.ThrustMax] = SimulationConstants.Searcher_Constants.ThrustMax,
                [UAVProperties.MaxCruiseSpeed] = SimulationConstants
                    .Searcher_Constants
                    .MaxCruiseSpeedKmph,
                [UAVProperties.MaxAcceleration] = SimulationConstants
                    .Searcher_Constants
                    .MaxAcceleration,
                [UAVProperties.MaxDeceleration] = SimulationConstants
                    .Searcher_Constants
                    .MaxDeceleration,
                [UAVProperties.FuelTankCapacity] = SimulationConstants
                    .Searcher_Constants
                    .FuelTankCapacity,
                [UAVProperties.FuelConsumption] = SimulationConstants
                    .Searcher_Constants
                    .FuelConsumption,

                [UAVProperties.TransmitPower] = SimulationConstants
                    .Searcher_Constants
                    .TransmitPowerDbm,
                [UAVProperties.TransmitAntennaGain] = SimulationConstants
                    .Searcher_Constants
                    .TransmitAntennaGainDbi,
                [UAVProperties.ReceiveAntennaGain] = SimulationConstants
                    .Searcher_Constants
                    .ReceiveAntennaGainDbi,
                [UAVProperties.TransmitLoss] = SimulationConstants
                    .Searcher_Constants
                    .TransmitLossDb,
                [UAVProperties.ReceiveLoss] = SimulationConstants.Searcher_Constants.ReceiveLossDb,
                [UAVProperties.Frequency] = SimulationConstants.Searcher_Constants.FrequencyHz,
                [UAVProperties.PropellerRadius] = SimulationConstants
                    .Searcher_Constants
                    .PropellerRadius,
            };
        }
    }
}
