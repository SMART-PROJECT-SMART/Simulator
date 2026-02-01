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

        public async Task<IEnumerable<int>> GetAvailableSleeveForUAVAsync(int tailId, CancellationToken cancellationToken = default)
        {
            string endpoint = string.Format(SimulationConstants.DeviceManagerApiEndpoints.GET_AVAILABLE_SLEEVE_FOR_UAV, tailId);
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<int>();
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<int>>(cancellationToken);
        }

        public async Task<bool> ReleaseSleeveByTailIdAsync(int tailId, CancellationToken cancellationToken = default)
        {
            string endpoint = string.Format(SimulationConstants.DeviceManagerApiEndpoints.RELEASE_SLEEVE_BY_TAIL_ID, tailId);
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
