using Simulation.Common.constants;
using Simulation.Dto.DeviceManager;

namespace Simulation.Services.DeviceManagerClient
{
    public class DeviceManagerClient : IDeviceManagerClient
    {
        private readonly HttpClient _httpClient;

        public DeviceManagerClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(SimulationConstants.HttpClients.DEVICE_MANAGER_HTTP_CLIENT);
        }

        public async Task<IEnumerable<DeviceManagerUAVDto>> GetAllUAVsAsync(CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(SimulationConstants.DeviceManagerApiEndpoints.GET_ALL_UAVS, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<DeviceManagerUAVDto>();
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<DeviceManagerUAVDto>>(cancellationToken);
        }
    }
}
