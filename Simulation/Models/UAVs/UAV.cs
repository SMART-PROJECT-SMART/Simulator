using Simulation.Common.Enums;

namespace Simulation.Models.UAVs;

public abstract class UAV
{
    public int TailId { get; set; }
    public double MaxAcceleration { get; set; }
    public double MaxDeceleration { get; set; }
    public double MaxCruiseSpeedKmph { get; set; }
    public double CruiseAltitude { get; set; }
    public double FuelTankSize { get; set; }
    public Dictionary<TelemetryFields,double> TelemetryData { get; set; }
    public string CurrentMissionId { get; set; }

    public UAV(Location startLocation,double fuealTankSize,double maxCruiseSpeed,double maxAcceleration,double maxDeceleration,int tailId) 
    {
        TailId = tailId;
        MaxAcceleration = maxAcceleration;
        MaxDeceleration = maxDeceleration;
        MaxCruiseSpeedKmph = maxCruiseSpeed;
        CruiseAltitude = startLocation.Altitude;
        FuelTankSize = fuealTankSize;
        SetTelemetryFields(startLocation);
    }

    private void SetTelemetryFields(Location startLocation)
    {
        TelemetryData = new Dictionary<TelemetryFields, double>();
        foreach (TelemetryFields field in Enum.GetValues(typeof(TelemetryFields)))
        {
            TelemetryData[field] = 0.0;
        }
        TelemetryData[TelemetryFields.LocationLatitude] = startLocation.Latitude;
        TelemetryData[TelemetryFields.LocationLongitude] = startLocation.Longitude;
        TelemetryData[TelemetryFields.LocationAltitudeAmsl] = startLocation.Altitude;
    }
}