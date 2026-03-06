using Simulation.Dto.DeviceManager;

namespace Simulation.Services.DeviceManagerClient
{
    public interface IDeviceManagerClient
    {
        Task<IEnumerable<DeviceManagerUAVDto>> GetAllUAVsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> GetAvailableSleeveForUAVAsync(int tailId, CancellationToken cancellationToken = default);
        Task<bool> ReleaseSleeveByTailIdAsync(int tailId, CancellationToken cancellationToken = default);
    }
}
