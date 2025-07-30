using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.helpers;

namespace Simulation.Models.UAVs;

public abstract class UAV
{
    public int TailId { get; set; }
    public Dictionary<TelemetryFields, double> TelemetryData { get; set; }
    public string CurrentMissionId { get; set; }

    public double FuelTankCapacity { get; set; }
    public double FuelConsumption { get; set; }

    public UAV(Location startLocation, int tailId, double fuelTankSize)
    {
        TailId = tailId;
        CurrentMissionId = string.Empty;
        TelemetryData = TelemetryFieldsHelper.FlightOnly();
        TelemetryData.SetLocation(startLocation);
        FuelTankCapacity = fuelTankSize;
        TelemetryData[TelemetryFields.FuelAmount] = fuelTankSize;
    }
}
