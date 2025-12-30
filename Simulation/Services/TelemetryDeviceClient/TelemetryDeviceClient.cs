using Simulation.Common.constants;
using Simulation.Dto.TelemetryDevice;
using Simulation.Models;

namespace Simulation.Services.TelemetryDeviceClient
{
    public class TelemetryDeviceClient : ITelemetryDeviceClient
    {
        private readonly HttpClient _httpClient;

        public TelemetryDeviceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(
                SimulationConstants.HttpClients.TELEMETRY_DEVICE_HTTP_CLIENT
            );
        }

        public async Task<bool> CreateTelemetryDeviceAsync(
            int tailId,
            IEnumerable<int> portNumbers,
            Location location,
            CancellationToken cancellationToken = default
        )
        {
            CreateTelemetryDeviceDto dto = new CreateTelemetryDeviceDto(
                tailId,
                portNumbers,
                location
            );

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                SimulationConstants.TelemetryDeviceApiEndpoints.ADD_TELEMETRY_DEVICE,
                dto,
                cancellationToken
            );

            return response.IsSuccessStatusCode;
        }
    }
}
