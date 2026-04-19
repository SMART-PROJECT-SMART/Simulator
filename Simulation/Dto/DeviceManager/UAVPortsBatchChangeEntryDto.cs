namespace Simulation.Dto.DeviceManager
{
    public class UAVPortsBatchChangeEntryDto
    {
        public int TailId { get; set; }
        public IEnumerable<int> NewPorts { get; set; } = [];
    }
}
