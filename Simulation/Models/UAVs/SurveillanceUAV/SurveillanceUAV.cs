using Simulation.Common.Enums;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public abstract class SurveillanceUAV : UAV
    {
        protected Dictionary<SensorType, bool> SensorsStatus { get; set; }
        protected double DataStorageCapacityGB { get;  set; }
        protected double UsedStorageGB { get; set; } = 0.0;

        protected SurveillanceUAV(
            Location startLocation,
            int tailId,
            Dictionary<SensorType, bool> sensorsStatus,
            double dataStorageCapacityGb
        ) : base(startLocation,tailId)
        {
            SensorsStatus = sensorsStatus;
            DataStorageCapacityGB = dataStorageCapacityGb;
        }
    }
}
