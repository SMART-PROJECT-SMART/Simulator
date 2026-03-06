using Core.Common.Enums;
using Simulation.Services.DeviceManagerClient;
using Simulation.Services.UAVManager;
using Simulation.Services.UAVStorage;

namespace Simulation.Services.UAVChangeHandlers
{
    public class UAVChangeHandlerFactory : IUAVChangeHandlerFactory
    {
        private readonly IUAVStorageService _uavStorageService;
        private readonly IDeviceManagerClient _deviceManagerClient;
        private readonly IUAVManager _uavManager;

        public UAVChangeHandlerFactory(IUAVStorageService uavStorageService, IDeviceManagerClient deviceManagerClient, IUAVManager uavManager)
        {
            _uavStorageService = uavStorageService;
            _deviceManagerClient = deviceManagerClient;
            _uavManager = uavManager;
        }

        public IUAVChangeHandler CreateHandler(CrudOperation operation)
        {
            return operation switch
            {
                CrudOperation.Created => new UAVCreatedHandler(_uavStorageService, _deviceManagerClient),
                CrudOperation.Updated => new UAVUpdatedHandler(_uavStorageService, _deviceManagerClient, _uavManager),
                CrudOperation.Deleted => new UAVDeletedHandler(_uavStorageService),
                _ => throw new ArgumentException($"Unsupported operation: {operation}", nameof(operation))
            };
        }
    }
}
