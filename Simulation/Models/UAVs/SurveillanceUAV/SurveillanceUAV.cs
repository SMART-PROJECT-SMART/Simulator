using System.Collections.Generic;
using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public abstract class SurveillanceUAV : UAV
    {
        public Dictionary<SensorType, bool> SensorStatus { get; set; }

        protected SurveillanceUAV(
            Location startLocation,
            int tailId,
            double fuelAmount,
            Dictionary<UAVProperties, double> properties,
            double dataStorageCapacityGB
        )
            : base(startLocation, tailId, fuelAmount, properties)
        {
            TelemetryData[TelemetryFields.DataStorageUsedGB] = 0.0;

            properties[UAVProperties.DataStorageCapacityGB] = dataStorageCapacityGB;
            SensorStatus = new Dictionary<SensorType, bool>();
        }
    }
}
