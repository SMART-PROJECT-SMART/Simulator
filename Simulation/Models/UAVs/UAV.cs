using Simulation.Common.Enums;

namespace Simulation.Models.UAVs;

public abstract class UAV
{
    public int TailId { get; set; }
    public Dictionary<TelemetryFields,double> TelemetryData { get; set; }
    public string CurrentMissionId { get; set; }

    public UAV(Location startLocation,int tailId) 
    {
        TailId = tailId;
        SetTelemetryFields(startLocation);
    }

    private void SetTelemetryFields(Location startLocation)
    {
        TelemetryData = new Dictionary<TelemetryFields, double>();
        foreach (TelemetryFields field in Enum.GetValues(typeof(TelemetryFields)))
        {
            TelemetryData[field] = 0.0;
        }
        TelemetryData[TelemetryFields.Latitude] = startLocation.Latitude;
        TelemetryData[TelemetryFields.longitude] = startLocation.Longitude;
        TelemetryData[TelemetryFields.Altitude] = startLocation.Altitude;
    }
}