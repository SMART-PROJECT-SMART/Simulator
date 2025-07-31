using System.Collections.Generic;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public abstract class SurveillanceUAV : UAV
    {
        public double DataStorageCapacityGB { get; set; }
        public Dictionary<SensorType, bool> SensorStatus { get; set; }

        protected SurveillanceUAV(
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
            double fuelConsumption,
            double dataStorageCapacityGB
        )
            : base(startLocation, tailId, mass, frontalSurface, wingsSurface, thrustMax, maxCruiseSpeed, maxAccelerationMps2, maxDecelerationMps2, fuelTankCapacity, fuelConsumption)
        {
            TelemetryData = TelemetryData;

            TelemetryData[TelemetryFields.DataStorageUsedGB] = 0.0;

            DataStorageCapacityGB = dataStorageCapacityGB;

            SensorStatus = new Dictionary<SensorType, bool>();
        }
    }
}