using Simulation.Common.Enums;
using Simulation.Services.helpers;

namespace Simulation.Models.UAVs.SurveillanceUAV
{
    public abstract class SurveillanceUAV : UAV
    {
        protected SurveillanceUAV(
            Location startLocation,
            int tailId
        ) : base(startLocation, tailId)
        {
            TelemetryData = TelemetryFieldsHelper.Initialize(TelemetryCategories.Flight, TelemetryCategories.Surveillance);
            TelemetryData.SetLocation(startLocation);
        }
    }
}
