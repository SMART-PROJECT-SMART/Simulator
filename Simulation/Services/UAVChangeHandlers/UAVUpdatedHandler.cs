using Simulation.Dto.DeviceManager;
using Simulation.Services.DeviceManagerClient;
using Simulation.Services.UAVStorage;

namespace Simulation.Services.UAVChangeHandlers
{
    public class UAVUpdatedHandler : IUAVChangeHandler
    {
        private readonly IUAVStorageService _uavStorageService;
        private readonly IDeviceManagerClient _deviceManagerClient;

        public UAVUpdatedHandler(IUAVStorageService uavStorageService, IDeviceManagerClient deviceManagerClient)
        {
            _uavStorageService = uavStorageService;
            _deviceManagerClient = deviceManagerClient;
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
                }
                _uavStorageService.AddOrUpdateUAV(uav);
            }
        }
    }
}
