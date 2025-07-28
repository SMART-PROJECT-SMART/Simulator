using Simulation.Common.Enums;

namespace Simulation.Dto.UAV
{
    public class CreateUAVDto
    {
        public int WingId { get; set; }
        public UAVTypes UAVType { get; set; }
        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }

        public string CurrentMissionId { get; set; } = string.Empty;

        public CreateUAVDto(int wingId, UAVTypes uavType, Dictionary<TelemetryFields, double> telemetryData = null, string currentMissionId = null)
        {
            WingId = wingId;
            UAVType = uavType;
            TelemetryData = telemetryData ?? new Dictionary<TelemetryFields, double>();
            CurrentMissionId = currentMissionId ?? string.Empty;
        }

        public CreateUAVDto() { }
    }
}
