using Simulation.Dto.Mission;
using Simulation.Models.Mission;
using Simulation.Ro.Mission;

namespace Simulation.Mappers
{
    public static class MissionMapper
    {
        public static Mission ToModel(this CreateMissionDto dto)
        {
            return new Mission
            {
                Destination = dto.Destination,
                WingId = dto.WingId
            };
        }

        public static MissionRo ToRo(this Mission mission)
        {
            return new MissionRo
            {
                Destination = mission.Destination,
                WingId = mission.WingId
            };
        }
    }
}
