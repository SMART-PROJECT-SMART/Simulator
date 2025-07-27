using Simulation.Common.Enums;
using Simulation.Dto.UAV;
using Simulation.Models;
using Simulation.Ro.UAV;

namespace Simulation.Mappers
{
    public static class UAVMapper
    {
        public static UAV toModel(this CreateUAVDto dto) {
            return new UAV
            {
                WingId = dto.WingId,
                UAVType = dto.UAVType,
                TelemetryData = dto.TelemetryData ?? new Dictionary<TelemetryFields, double>(),
                CurrentMissionId = dto.CurrentMissionId ?? string.Empty
            };
        }

        public static UAVRo toRo(this UAV uav) {
            return new UAVRo
            {
                WingId = uav.WingId,
                UAVType = uav.UAVType,
                TelemetryData = uav.TelemetryData ?? new Dictionary<TelemetryFields, double>(),
                CurrentMissionId = uav.CurrentMissionId ?? string.Empty
            };
        }
    }
}
