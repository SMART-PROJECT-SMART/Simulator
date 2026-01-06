namespace Simulation.Services.MissionServiceClient
{
    public interface IMissionServiceClient
    {
        Task<bool> NotifyMissionCompletedAsync(
            int tailId,
            CancellationToken cancellationToken = default
        );
    }
}
