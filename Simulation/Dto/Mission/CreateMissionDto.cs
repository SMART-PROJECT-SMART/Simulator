using Simulation.Models.Mission;

namespace Simulation.Dto.Mission
{
    public class CreateMissionDto
    {
        public Location Destination { get; set; }
        public int WingId { get; set; }

        public CreateMissionDto() { }
        public CreateMissionDto(Location destination, int wingId)
        {
            Destination = destination;
            WingId = wingId;
        }
    }
}
