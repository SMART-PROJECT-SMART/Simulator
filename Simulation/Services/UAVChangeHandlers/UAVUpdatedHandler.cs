using Simulation.Dto.DeviceManager;
using Simulation.Services.DeviceManagerClient;
using Simulation.Services.UAVManager;
using Simulation.Services.UAVStorage;

namespace Simulation.Services.UAVChangeHandlers
{
    public class UAVUpdatedHandler : IUAVChangeHandler
    {
        private readonly IUAVStorageService _uavStorageService;
        private readonly IDeviceManagerClient _deviceManagerClient;
        private readonly IUAVManager _uavManager;

        public UAVUpdatedHandler(IUAVStorageService uavStorageService, IDeviceManagerClient deviceManagerClient, IUAVManager uavManager)
        {
            _uavStorageService = uavStorageService;
            _deviceManagerClient = deviceManagerClient;
            _uavManager = uavManager;
        }

        public async Task HandleUAVChangeAsync(int tailId, int? newTailId = null, CancellationToken cancellationToken = default)
        {
            int effectiveTailId = newTailId ?? tailId;

            IEnumerable<DeviceManagerUAVDto> allUAVs = await _deviceManagerClient.GetAllUAVsAsync(cancellationToken);
            DeviceManagerUAVDto uav = allUAVs.FirstOrDefault(u => u.TailId == effectiveTailId);

            if (uav != null)
            {
                if (newTailId.HasValue && newTailId.Value != tailId)
                {
                    _uavStorageService.RemoveUAV(tailId);
                    _uavManager.UpdateTailId(tailId, newTailId.Value);
                }
                _uavStorageService.AddOrUpdateUAV(uav);
            }
        }
    }
}
