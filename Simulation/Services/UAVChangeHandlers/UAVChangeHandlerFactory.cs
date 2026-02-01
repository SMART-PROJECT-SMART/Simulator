using Core.Common.Enums;
using Simulation.Services.DeviceManagerClient;
using Simulation.Services.UAVStorage;

namespace Simulation.Services.UAVChangeHandlers
{
    public class UAVChangeHandlerFactory
    {
        private readonly IUAVStorageService _uavStorageService;
        private readonly IDeviceManagerClient _deviceManagerClient;

        public UAVChangeHandlerFactory(IUAVStorageService uavStorageService, IDeviceManagerClient deviceManagerClient)
        {
            _uavStorageService = uavStorageService;
            _deviceManagerClient = deviceManagerClient;
        }

        public IUAVChangeHandler CreateHandler(CrudOperation operation)
        {
            return operation switch
            {
                CrudOperation.Created => new UAVCreatedHandler(_uavStorageService, _deviceManagerClient),
                CrudOperation.Updated => new UAVUpdatedHandler(_uavStorageService, _deviceManagerClient),
                CrudOperation.Deleted => new UAVDeletedHandler(_uavStorageService),
                _ => throw new ArgumentException($"Unsupported operation: {operation}", nameof(operation))
            };
        }
    }
}
