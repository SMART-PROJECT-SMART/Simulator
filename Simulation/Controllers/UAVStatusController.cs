using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simulation.Dto.DeviceManager;
using Simulation.Models.UAVs;
using Simulation.Services.UAVManager;
using Simulation.Services.UAVStorage;
using System.Threading.Tasks;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UAVStatusController : ControllerBase
    {
        private readonly IUAVManager _uavManager;
        private readonly IUAVStorageService _uavStorageService;
        public UAVStatusController(IUAVManager uavManager,IUAVStorageService storageService)
        {
            _uavManager = uavManager;
            _uavStorageService = storageService;
        }
        [HttpGet("all-uav")]
        public async Task<IActionResult> GetlAllUAV() {
            IEnumerable<DeviceManagerUAVDto> uavs = _uavStorageService.GetAllUAVs();
            foreach (var uav in uavs)
            {
                Console.WriteLine($"TailId: {uav.TailId}, PlatformType: {uav.PlatformType}, BaseLocation: {uav.BaseLocation}");
            }
            return Ok(uavs);
        }
        [HttpGet("active-uav")]
        public IActionResult GetActiveUAVs()
        {
            IEnumerable<int> activeTailIds = _uavManager.GetActiveTailIds;
            return Ok(activeTailIds);
        }
    }
}
