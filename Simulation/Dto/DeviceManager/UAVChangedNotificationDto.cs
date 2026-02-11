using Core.Common.Enums;

namespace Simulation.Dto.DeviceManager
{
    public class UAVChangedNotificationDto
    {
        public UAVChangedNotificationDto(CrudOperation operation, int tailId, int? newTailId = null)
        {
            Operation = operation;
            TailId = tailId;
            NewTailId = newTailId;
        }

        public CrudOperation Operation { get; set; }
        public int TailId { get; set; }
        public int? NewTailId { get; set; }
    }
}
