using Simulation.Common.Enums;
using Simulation.Services.helpers;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        protected ArmedUAV(
            Location startLocation,
            int tailId,
            double fuelTankSize,
            double fuelConsumption
        )
            : base(startLocation, tailId, fuelTankSize, fuelConsumption)
        {
            TelemetryData = TelemetryFieldsHelper.Initialize(
                TelemetryCategories.Flight,
                TelemetryCategories.Armed
            );
            TelemetryData.SetLocation(startLocation);
        }
    }
}
