namespace Simulation.Dto.DeviceManager
{
    public class UAVPortsChangedBatchNotificationDto
    {
        public IEnumerable<UAVPortsBatchChangeEntryDto> Changes { get; set; } = [];
    }
}
