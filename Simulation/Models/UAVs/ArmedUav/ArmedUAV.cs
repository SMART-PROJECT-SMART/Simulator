using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.helpers;

namespace Simulation.Models.UAVs.ArmedUav
{
    public abstract class ArmedUAV : UAV
    {
        protected ArmedUAV(
            Location startLocation,
            int tailId
        ) : base(startLocation, tailId)
        {
            TelemetryData = TelemetryFieldsHelper.Initialize(TelemetryCategories.Flight, TelemetryCategories.Armed);
            TelemetryData.SetLocation(startLocation);
        }
    }
}
