namespace Simulation.Services.Quartz
{
    public interface IQuartzManager
    {
        Task<bool> ScheduleUAVFlightPathJob(int uavId, int intervalSeconds);
        Task<bool> DeleteUAVFlightPathJob(int uavId);
        Task<bool> PauseUAVFlightPathJob(int uavId);
        Task<bool> ResumeUAVFlightPathJob(int uavId);
        Task<bool> IsJobScheduled(int uavId);
        Task<IReadOnlyList<string>> GetActiveJobIds();
        Task<int> GetActiveJobCount();
        Task ShutdownScheduler();
        Task StartScheduler();
        Task<bool> DeleteAllJobs();
    }
}
