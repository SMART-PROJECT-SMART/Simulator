using Simulation.Common.Enums;
using Simulation.Services.helpers;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public abstract class SurveillanceUAV : UAV
    {
        protected SurveillanceUAV(Location startLocation, int tailId,double fuelTankSize,double fuelConsumption)
            : base(startLocation, tailId,fuelTankSize, fuelConsumption)
        {
            TelemetryData = TelemetryFieldsHelper.Initialize(
                TelemetryCategories.Flight,
                TelemetryCategories.Surveillance
            );
            TelemetryData.SetLocation(startLocation);
        }
    }
}
