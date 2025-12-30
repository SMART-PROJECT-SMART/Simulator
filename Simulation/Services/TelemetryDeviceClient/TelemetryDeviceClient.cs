using Simulation.Common.constants;
using Simulation.Dto.TelemetryDevice;
using Simulation.Models;

namespace Simulation.Services.TelemetryDeviceClient
{
    public class TelemetryDeviceClient : ITelemetryDeviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TelemetryDeviceClient> _logger;

        public TelemetryDeviceClient(
            IHttpClientFactory httpClientFactory,
            ILogger<TelemetryDeviceClient> logger
        )
        {
            _httpClient = httpClientFactory.CreateClient(
                SimulationConstants.HttpClients.TELEMETRY_DEVICE_HTTP_CLIENT
            );
            _logger = logger;
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

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                    SimulationConstants.TelemetryDeviceApiEndpoints.ADD_TELEMETRY_DEVICE,
                    dto,
                    cancellationToken
                );

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "Failed to create telemetry device. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode,
                        errorContent
                    );
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception while creating telemetry device for TailId {TailId}",
                    tailId
                );
                return false;
            }
        }
    }
}
