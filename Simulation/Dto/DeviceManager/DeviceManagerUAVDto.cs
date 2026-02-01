using Core.Common.Enums;
using Core.Models;

namespace Simulation.Dto.DeviceManager
{
    public class DeviceManagerUAVDto
    {
        public DeviceManagerUAVDto(int tailId, PlatformType platformType, Location baseLocation)
        {
            TailId = tailId;
            PlatformType = platformType;
            BaseLocation = baseLocation;
        }

        public int TailId { get; set; }
        public PlatformType PlatformType { get; set; }
        public Location BaseLocation { get; set; }
    }
}
