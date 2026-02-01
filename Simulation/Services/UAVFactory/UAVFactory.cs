using Core.Models;
using Core.Common.Enums;
using Simulation.Dto.DeviceManager;
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Models.UAVs.ArmedUav;
using Simulation.Models.UAVs.SurveillanceUAV;

namespace Simulation.Services.UAVFactory
{
    public class UAVFactory : IUAVFactory
    {
        public UAV CreateUAV(DeviceManagerUAVDto uavDto, Location startLocation)
        {
            return uavDto.PlatformType switch
            {
                PlatformType.Hermes900 => new Hermes900(uavDto.TailId, startLocation),
                PlatformType.HeronTP => new HeronTp(uavDto.TailId, startLocation),
                PlatformType.Hermes450 => new Hermes450(uavDto.TailId, startLocation),
                PlatformType.Searcher => new Searcher(uavDto.TailId, startLocation),
                _ => throw new ArgumentException($"Unsupported platform type: {uavDto.PlatformType}", nameof(uavDto))
            };
        }
    }
}
