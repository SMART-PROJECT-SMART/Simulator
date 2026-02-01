using Core.Common.Enums;

namespace Simulation.Dto.DeviceManager
{
    public class UAVChangedNotificationDto
    {
        public UAVChangedNotificationDto(CrudOperation operation, int tailId)
        {
            Operation = operation;
            TailId = tailId;
        }

        public CrudOperation Operation { get; set; }
        public int TailId { get; set; }
    }
}
