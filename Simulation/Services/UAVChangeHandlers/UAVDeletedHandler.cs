using Simulation.Services.UAVStorage;

namespace Simulation.Services.UAVChangeHandlers
{
    public class UAVDeletedHandler : IUAVChangeHandler
    {
        private readonly IUAVStorageService _uavStorageService;

        public UAVDeletedHandler(IUAVStorageService uavStorageService)
        {
            _uavStorageService = uavStorageService;
        }

        public Task HandleUAVChangeAsync(int tailId, int? newTailId = null, CancellationToken cancellationToken = default)
        {
            _uavStorageService.RemoveUAV(tailId);
            return Task.CompletedTask;
        }
    }
}
