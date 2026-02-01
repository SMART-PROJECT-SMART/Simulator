using System.Collections.Concurrent;
using Simulation.Dto.DeviceManager;
using Simulation.Services.DeviceManagerClient;

namespace Simulation.Services.UAVStorage
{
    public class UAVStorageService : IUAVStorageService
    {
        private readonly ConcurrentDictionary<int, DeviceManagerUAVDto> _uavs;
        private readonly IDeviceManagerClient _deviceManagerClient;

        public UAVStorageService(IDeviceManagerClient deviceManagerClient)
        {
            _uavs = new ConcurrentDictionary<int, DeviceManagerUAVDto>();
            _deviceManagerClient = deviceManagerClient;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<DeviceManagerUAVDto> uavs = await _deviceManagerClient.GetAllUAVsAsync(cancellationToken);

            foreach (DeviceManagerUAVDto uav in uavs)
            {
                _uavs.TryAdd(uav.TailId, uav);
            }
        }

        public void AddOrUpdateUAV(DeviceManagerUAVDto uav)
        {
            _uavs.AddOrUpdate(uav.TailId, uav, (key, existing) => uav);
        }

        public void RemoveUAV(int tailId)
        {
            _uavs.TryRemove(tailId, out _);
        }

        public DeviceManagerUAVDto GetUAV(int tailId)
        {
            _uavs.TryGetValue(tailId, out DeviceManagerUAVDto uav);
            return uav;
        }

        public IEnumerable<DeviceManagerUAVDto> GetAllUAVs()
        {
            return _uavs.Values;
        }
    }
}
