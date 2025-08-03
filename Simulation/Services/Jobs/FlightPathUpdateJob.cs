using Quartz;
using Simulation.Common.constants;
using Simulation.Services;

public class FlightPathUpdateJob : IJob
{
    private readonly UAVManager _uavManager;

    public FlightPathUpdateJob(UAVManager uavManager)
    {
        _uavManager = uavManager;
    }

    public Task Execute(IJobExecutionContext jobExecutionContext)
    {
        var uavId = jobExecutionContext.JobDetail.JobDataMap.GetInt(SimulationConstants.Quartz.UAV_ID);
        var context = _uavManager.GetUAVContext(uavId);

        if (context?.Service == null)
            return Task.CompletedTask;

        context.Service.UpdateLocation();

        return Task.CompletedTask;
    }
}