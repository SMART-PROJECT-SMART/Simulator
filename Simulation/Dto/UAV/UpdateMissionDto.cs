namespace Simulation.Dto.UAV
{
    public class UpdateMissionDto
    {
        public string MissionId { get; set; }
        public string WingId { get; set; }

        public UpdateMissionDto(string missionId, string wingId)
        {
            MissionId = missionId;
            WingId = wingId;
        }
    }
}
