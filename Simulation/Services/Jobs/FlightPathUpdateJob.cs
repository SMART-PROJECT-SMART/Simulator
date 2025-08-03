using Quartz;
using Simulation.Common.constants;
using Simulation.Services.UAVManager;

public class FlightPathUpdateJob : IJob
{
    private readonly IUAVManager _uavManager;

    public FlightPathUpdateJob(IUAVManager uavManager)
    {
        _uavManager = uavManager;
    }

    public Task Execute(IJobExecutionContext jobExecutionContext)
    {
        var uavId = jobExecutionContext.JobDetail.JobDataMap.GetInt(SimulationConstants.Quartz.UAV_ID);
        var context = _uavManager.GetUAVContext(uavId);

        context?.Service?.UpdateLocation();

        return Task.CompletedTask;
    }
}