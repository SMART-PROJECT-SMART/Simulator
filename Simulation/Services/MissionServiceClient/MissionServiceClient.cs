using Simulation.Common.constants;

namespace Simulation.Services.MissionServiceClient
{
    public class MissionServiceClient : IMissionServiceClient
    {
        private readonly HttpClient _httpClient;

        public MissionServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(
                SimulationConstants.HttpClients.MISSION_SERVICE_HTTP_CLIENT
            );
        }

        public async Task<bool> NotifyMissionCompletedAsync(
            int tailId,
            CancellationToken cancellationToken = default
        )
        {
            string endpoint = $"{SimulationConstants.MissionServiceApiEndpoints.MISSION_COMPLETED}/{tailId}";

            HttpResponseMessage response = await _httpClient.PostAsync(
                endpoint,
                null,
                cancellationToken
            );

            return response.IsSuccessStatusCode;
        }
    }
}
