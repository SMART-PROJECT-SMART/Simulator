using Core.Models;
﻿using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Channels;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Models.UAVs
{
    public abstract class UAV
    {
        public int TailId { get; set; }
        public Dictionary<UAVProperties, double> Properties { get; set; }
        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }
        public string CurrentMissionId { get; set; }
        public List<Channel> Channels { get; set; }

        protected UAV(
            Location startLocation,
            int tailId,
            double fuelAmount,
            Dictionary<UAVProperties, double> properties,
            List<Channel> channels
        )
        {
            TailId = tailId;
            Properties = properties;

            CurrentMissionId = string.Empty;
            Channels = channels;
            double fuelTankCapacity = Properties[UAVProperties.FuelTankCapacity];
            double fuelAmountPercentage = fuelTankCapacity <= 0.0
                ? SimulationConstants.TelemetryData.EMPTY_PERCENTAGE
                : fuelAmount * SimulationConstants.TelemetryData.FULL_PERCENTAGE / fuelTankCapacity;

            TelemetryData = new Dictionary<TelemetryFields, double>
            {
                [TelemetryFields.Latitude] = startLocation.Latitude,
                [TelemetryFields.Longitude] = startLocation.Longitude,
                [TelemetryFields.Altitude] = startLocation.Altitude,
                [TelemetryFields.FuelAmount] = fuelAmountPercentage,
                [TelemetryFields.AmmoPercentage] = 0.0,
                [TelemetryFields.ThrottlePercent] = 0.0,
                [TelemetryFields.CurrentSpeedKmph] = 0.0,
                [TelemetryFields.YawDeg] = 0.0,
                [TelemetryFields.PitchDeg] = 0.0,
                [TelemetryFields.RollDeg] = 0.0,
                [TelemetryFields.LandingGearStatus] = SimulationConstants.TelemetryData.WHEELS_DOWN,
                [TelemetryFields.FlightTimeSec] = 0.0,
                [TelemetryFields.SignalStrength] = 0.0,
                [TelemetryFields.EngineDegrees] = 0.0,
                [TelemetryFields.NearestSleeveId] = 0,
                [TelemetryFields.MissionId] = 0,
            };
        }

        public void ConsumeFuel(double deltaSec)
        {
            TelemetryData.TryGetValue(TelemetryFields.ThrottlePercent, out double throttlePct);
            double thrust = Properties[UAVProperties.ThrustMax] * (throttlePct / 100.0);

            double burnedInKg = thrust * Properties[UAVProperties.FuelConsumption] * deltaSec;
            double fuelTankCapacity = Properties[UAVProperties.FuelTankCapacity];
            if (fuelTankCapacity <= 0.0)
            {
                TelemetryData[TelemetryFields.FuelAmount] = SimulationConstants.TelemetryData.EMPTY_PERCENTAGE;
                return;
            }

            double remainingFuelPercentage = TelemetryData[TelemetryFields.FuelAmount];
            double remainingFuelKg = remainingFuelPercentage * fuelTankCapacity / SimulationConstants.TelemetryData.FULL_PERCENTAGE;
            remainingFuelKg = Math.Max(remainingFuelKg - burnedInKg, 0.0);
            double nextFuelPercentage = remainingFuelKg * SimulationConstants.TelemetryData.FULL_PERCENTAGE / fuelTankCapacity;
            TelemetryData[TelemetryFields.FuelAmount] = nextFuelPercentage;
        }

        public Location GetLocation()
        {
            return new Location(
                TelemetryData[TelemetryFields.Latitude],
                TelemetryData[TelemetryFields.Longitude],
                TelemetryData[TelemetryFields.Altitude]
            );
        }

        public void TakeOff() =>
            TelemetryData[TelemetryFields.LandingGearStatus] = SimulationConstants
                .TelemetryData
                .WHEELS_UP;

        public void Land() =>
            TelemetryData[TelemetryFields.LandingGearStatus] = SimulationConstants
                .TelemetryData
                .WHEELS_DOWN;

        public void UpdateRpm()
        {
            TelemetryData[TelemetryFields.Rpm] =
                TelemetryData[TelemetryFields.CurrentSpeedKmph].ToKmhFromMps()
                / Properties[UAVProperties.PropellerRadius]
                * 60;
            UpdateEngineDegrees();
        }

        public void UpdateEngineDegrees()
        {
            TelemetryData[TelemetryFields.EngineDegrees] =
                TelemetryData[TelemetryFields.Rpm] / 60.0 * 360.0;
        }
    }
}
