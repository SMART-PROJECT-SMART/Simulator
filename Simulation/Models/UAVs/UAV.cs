using Simulation.Common.Enums;

namespace Simulation.Models.UAVs
{
    public abstract class UAV
    {
        public int TailId { get; set; }
        public double Mass { get; set; }
        public double FrontalSurface { get; set; }
        public double WingsSurface { get; set; }
        public double ThrustMax { get; set; }
        public double MaxCruiseSpeed { get; set; }
        public double MaxAccelerationMps2 { get; set; }
        public double MaxDecelerationMps2 { get; set; }
        public double FuelTankCapacity { get; set; }
        public double FuelConsumption { get; set; }

        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }
        public string CurrentMissionId { get; set; }

        protected UAV(
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
        {
            TailId = tailId;
            Mass = mass;
            FrontalSurface = frontalSurface;
            WingsSurface = wingsSurface;
            ThrustMax = thrustMax;
            MaxCruiseSpeed = maxCruiseSpeed;
            MaxAccelerationMps2 = maxAccelerationMps2;
            MaxDecelerationMps2 = maxDecelerationMps2;
            FuelTankCapacity = fuelTankCapacity;
            FuelConsumption = fuelConsumption;

            CurrentMissionId = string.Empty;

            TelemetryData = new Dictionary<TelemetryFields, double>();

            TelemetryData[TelemetryFields.Latitude] = startLocation.Latitude;
            TelemetryData[TelemetryFields.Longitude] = startLocation.Longitude;
            TelemetryData[TelemetryFields.Altitude] = startLocation.Altitude;

            TelemetryData[TelemetryFields.FuelAmount] = fuelTankCapacity;
            TelemetryData[TelemetryFields.ThrottlePercent] = 0.0;
            TelemetryData[TelemetryFields.CurrentSpeedKmph] = 0.0;
            TelemetryData[TelemetryFields.YawDeg] = 0.0;
            TelemetryData[TelemetryFields.PitchDeg] = 0.0;
            TelemetryData[TelemetryFields.RollDeg] = 0.0;
        }

        public void ConsumeFuel(double deltaSec)
        {
            TelemetryData.TryGetValue(TelemetryFields.ThrottlePercent, out double throttlePct);
            double thrust = ThrustMax * (throttlePct / 100.0);

            double burnedInKg = thrust * FuelConsumption * deltaSec;

            double remainingFuel = TelemetryData[TelemetryFields.FuelAmount];
            remainingFuel = Math.Max(remainingFuel - burnedInKg, 0.0);
            TelemetryData[TelemetryFields.FuelAmount] = remainingFuel;
        }
    }
}
