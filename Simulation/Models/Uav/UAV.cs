using Simulation.Common.Enums;

public class UAV
{
    public string Id { get; set; }
    public int WingId { get; set; }
    public UAVTypes UAVType { get; set; }
    public double MaxAcceleration { get; set; }
    public double MaxDeceleration { get; set; }
    public double MaxCruiseSpeedKmph { get; set; }
    public Dictionary<TelemetryFields,double> TelemetryData { get; set; }
    public string CurrentMissionId { get; set; }
}
