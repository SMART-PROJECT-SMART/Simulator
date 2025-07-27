using Simulation.Common.Enums;

namespace Simulation.Ro.UAV
{
    public class UAVRo
    {
        public int WingId { get; set; }

        public UAVTypes UAVType { get; set; }

        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }

        public string CurrentMissionId { get; set; }

        public UAVRo() { }

        public UAVRo(int wingId, UAVTypes uAVType, Dictionary<TelemetryFields, double> telemetryData, string currentMissionId)
        {
            WingId = wingId;
            UAVType = uAVType;
            TelemetryData = telemetryData;
            CurrentMissionId = CurrentMissionId;
        }
    }
}
