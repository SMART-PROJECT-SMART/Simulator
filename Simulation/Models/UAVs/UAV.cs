using Simulation.Common.Enums;

namespace Simulation.Models.UAVs
{
    public abstract class UAV
    {
        public int TailId { get; set; }
        public Dictionary<UAVProperties, double> Properties { get; set; }
        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }
        public string CurrentMissionId { get; set; }

        protected UAV(
            Location startLocation,
            int tailId,
            double fuelAmount,
            Dictionary<UAVProperties, double> properties
        )
        {
            TailId = tailId;
            Properties = properties;

            CurrentMissionId = string.Empty;

            TelemetryData = new Dictionary<TelemetryFields, double>
            {
                [TelemetryFields.Latitude] = startLocation.Latitude,
                [TelemetryFields.Longitude] = startLocation.Longitude,
                [TelemetryFields.Altitude] = startLocation.Altitude,
            };

            TelemetryData[TelemetryFields.FuelAmount] = fuelAmount;

            TelemetryData[TelemetryFields.ThrottlePercent] = 0.0;
            TelemetryData[TelemetryFields.CurrentSpeedKmph] = 0.0;
            TelemetryData[TelemetryFields.YawDeg] = 0.0;
            TelemetryData[TelemetryFields.PitchDeg] = 0.0;
            TelemetryData[TelemetryFields.RollDeg] = 0.0;
        }

        public void ConsumeFuel(double deltaSec)
        {
            TelemetryData.TryGetValue(TelemetryFields.ThrottlePercent, out double throttlePct);
            double thrust = Properties[UAVProperties.ThrustMax] * (throttlePct / 100.0);

            double burnedInKg = thrust * Properties[UAVProperties.FuelConsumption] * deltaSec;

            double remainingFuel = TelemetryData[TelemetryFields.FuelAmount];
            remainingFuel = Math.Max(remainingFuel - burnedInKg, 0.0);
            TelemetryData[TelemetryFields.FuelAmount] = remainingFuel;
        }
    }
}
