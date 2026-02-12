namespace Simulation.Dto.DeviceManager
{
    public class UAVPortsChangedNotificationDto
    {
        public UAVPortsChangedNotificationDto(int tailId, IEnumerable<int> newPorts)
        {
            TailId = tailId;
            NewPorts = newPorts;
        }

        public int TailId { get; set; }
        public IEnumerable<int> NewPorts { get; set; }
    }
}
