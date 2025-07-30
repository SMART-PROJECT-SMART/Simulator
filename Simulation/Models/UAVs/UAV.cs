using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.helpers;
using System;
using System.Collections.Generic;

namespace Simulation.Models.UAVs
{
    public abstract class UAV
    {
        public int TailId { get; set; }
        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }
        public string CurrentMissionId { get; set; }

        public double FuelTankCapacity { get; set; }
        public double FuelConsumption { get; set; }

        protected UAV(Location startLocation, int tailId, double fuelTankSize, double fuelConsumption)
        {
            TailId = tailId;
            CurrentMissionId = string.Empty;
            TelemetryData = TelemetryFieldsHelper.FlightOnly();
            TelemetryData.SetLocation(startLocation);

            FuelTankCapacity = fuelTankSize;
            FuelConsumption = fuelConsumption;
            TelemetryData[TelemetryFields.FuelAmount] = fuelTankSize;
            TelemetryData[TelemetryFields.ThrottlePercent] = 0.0;
        }

        public void ConsumeFuel(double deltaSec)
        {
            TelemetryData.TryGetValue(TelemetryFields.ThrottlePercent, out double throttlePct);
            TelemetryData.TryGetValue(TelemetryFields.ThrustMax, out double maxThrust);
            double thrust = maxThrust * (throttlePct / 100.0);

            double burnedInKg = thrust * FuelConsumption * deltaSec;

            double remainingFuel = TelemetryData[TelemetryFields.FuelAmount];
            remainingFuel = Math.Max(remainingFuel - burnedInKg, 0.0);
            TelemetryData[TelemetryFields.FuelAmount] = remainingFuel;
        }
    }
}