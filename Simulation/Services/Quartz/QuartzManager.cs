using Quartz;
using Quartz.Impl.Matchers;
using Simulation.Common.constants;
using Simulation.Services.Jobs;

namespace Simulation.Services.Quartz
{
    public class QuartzManager : IQuartzManager
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger<QuartzManager> _logger;

        public QuartzManager(IScheduler scheduler, ILogger<QuartzManager> logger)
        {
            _scheduler = scheduler;
            _logger = logger;
        }

        public async Task<bool> ScheduleUAVFlightPathJob(int uavId, int intervalSeconds)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");
                var triggerKey = new TriggerKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");

                if (await _scheduler.CheckExists(jobKey))
                {
                    _logger.LogWarning("Job for UAV {UavId} already exists", uavId);
                    return false;
                }

                var jobDetail = JobBuilder.Create<FlightPathUpdateJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData(SimulationConstants.Quartz.UAV_ID, uavId)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(intervalSeconds)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);
                
                _logger.LogInformation("Successfully scheduled flight path job for UAV {UavId} with {IntervalSeconds}s interval", 
                    uavId, intervalSeconds);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule job for UAV {UavId}", uavId);
                return false;
            }
        }

        public async Task<bool> DeleteUAVFlightPathJob(int uavId)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");
                
                var deleted = await _scheduler.DeleteJob(jobKey);
                
                if (deleted)
                {
                    _logger.LogInformation("Successfully deleted flight path job for UAV {UavId}", uavId);
                }
                else
                {
                    _logger.LogWarning("Job for UAV {UavId} was not found or could not be deleted", uavId);
                }
                
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete job for UAV {UavId}", uavId);
                return false;
            }
        }

        public async Task<bool> PauseUAVFlightPathJob(int uavId)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");
                
                if (!await _scheduler.CheckExists(jobKey))
                {
                    _logger.LogWarning("Job for UAV {UavId} does not exist, cannot pause", uavId);
                    return false;
                }

                await _scheduler.PauseJob(jobKey);
                _logger.LogInformation("Successfully paused flight path job for UAV {UavId}", uavId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pause job for UAV {UavId}", uavId);
                return false;
            }
        }

        public async Task<bool> ResumeUAVFlightPathJob(int uavId)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");
                
                if (!await _scheduler.CheckExists(jobKey))
                {
                    _logger.LogWarning("Job for UAV {UavId} does not exist, cannot resume", uavId);
                    return false;
                }

                await _scheduler.ResumeJob(jobKey);
                _logger.LogInformation("Successfully resumed flight path job for UAV {UavId}", uavId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resume job for UAV {UavId}", uavId);
                return false;
            }
        }

        public async Task<bool> IsJobScheduled(int uavId)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");
                return await _scheduler.CheckExists(jobKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if job exists for UAV {UavId}", uavId);
                return false;
            }
        }

        public async Task<IReadOnlyList<string>> GetActiveJobIds()
        {
            try
            {
                var jobGroups = await _scheduler.GetJobGroupNames();
                var activeJobs = new List<string>();

                foreach (var group in jobGroups)
                {
                    var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                    foreach (var jobKey in jobKeys)
                    {
                        if (jobKey.Name.StartsWith(SimulationConstants.Quartz.IDENTITY_PREFIX))
                        {
                            activeJobs.Add(jobKey.Name);
                        }
                    }
                }

                return activeJobs.AsReadOnly();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active job IDs");
                return new List<string>().AsReadOnly();
            }
        }

        public async Task<int> GetActiveJobCount()
        {
            try
            {
                var activeJobs = await GetActiveJobIds();
                return activeJobs.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active job count");
                return 0;
            }
        }

        public async Task ShutdownScheduler()
        {
            try
            {
                if (!_scheduler.IsShutdown)
                {
                    await _scheduler.Shutdown(waitForJobsToComplete: true);
                    _logger.LogInformation("Scheduler shutdown completed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to shutdown scheduler");
            }
        }

        public async Task StartScheduler()
        {
            try
            {
                if (_scheduler.IsShutdown)
                {
                    _logger.LogWarning("Cannot start a shutdown scheduler");
                    return;
                }

                if (!_scheduler.IsStarted)
                {
                    await _scheduler.Start();
                    _logger.LogInformation("Scheduler started successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start scheduler");
            }
        }

        public async Task<bool> DeleteAllJobs()
        {
            try
            {
                var jobGroups = await _scheduler.GetJobGroupNames();
                var deletedCount = 0;

                foreach (var group in jobGroups)
                {
                    var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                    foreach (var jobKey in jobKeys.Where(jk => jk.Name.StartsWith(SimulationConstants.Quartz.IDENTITY_PREFIX)))
                    {
                        var deleted = await _scheduler.DeleteJob(jobKey);
                        if (deleted) deletedCount++;
                    }
                }

                _logger.LogInformation("Deleted {DeletedCount} UAV flight path jobs", deletedCount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete all jobs");
                return false;
            }
        }
    }
}
