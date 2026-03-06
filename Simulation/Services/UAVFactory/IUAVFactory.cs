using Core.Models;
using Simulation.Dto.DeviceManager;
using Simulation.Models;
using Simulation.Models.UAVs;

namespace Simulation.Services.UAVFactory
{
    public interface IUAVFactory
    {
        UAV CreateUAV(DeviceManagerUAVDto uavDto, Location startLocation);
    }
}
