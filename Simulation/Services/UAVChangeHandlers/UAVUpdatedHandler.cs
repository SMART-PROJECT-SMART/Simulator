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

        public async Task HandleUAVChangeAsync(int tailId, CancellationToken cancellationToken = default)
        {
            IEnumerable<DeviceManagerUAVDto> allUAVs = await _deviceManagerClient.GetAllUAVsAsync(cancellationToken);
            DeviceManagerUAVDto uav = allUAVs.FirstOrDefault(u => u.TailId == tailId);

            if (uav != null)
            {
                _uavStorageService.AddOrUpdateUAV(uav);
            }
        }
    }
}
