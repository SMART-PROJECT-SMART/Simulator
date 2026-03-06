using Simulation.Dto.DeviceManager;

namespace Simulation.Services.UAVStorage
{
    public interface IUAVStorageService
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
        void AddOrUpdateUAV(DeviceManagerUAVDto uav);
        void RemoveUAV(int tailId);
        DeviceManagerUAVDto GetUAV(int tailId);
        IEnumerable<DeviceManagerUAVDto> GetAllUAVs();
    }
}
