using Simulation.Models.Mission;

namespace Simulation.Ro.Mission
{
    public class MissionRo
    {
        public Location Destination { get; set; }
        public int WingId { get; set; }

        public MissionRo() { }

        public MissionRo(Location destination, int wingId)
        {
            Destination = destination;
            WingId = wingId;
        } 
    }
}
