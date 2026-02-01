using Simulation.Dto.DeviceManager;

namespace Simulation.Services.DeviceManagerClient
{
    public interface IDeviceManagerClient
    {
        Task<IEnumerable<DeviceManagerUAVDto>> GetAllUAVsAsync(CancellationToken cancellationToken = default);
    }
}
