using Quartz;
using Quartz.Impl.Matchers;
using Simulation.Common.constants;
using Simulation.Services.Quartz.Jobs;

namespace Simulation.Services.Quartz
{
    public class QuartzFlightJobManager : IQuartzFlightJobManager
    {
        private readonly IScheduler _scheduler;

        public QuartzFlightJobManager(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task<bool> ScheduleUAVFlightPathJob(int uavId, int intervalSeconds)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");
                var triggerKey = new TriggerKey(
                    $"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}"
                );

                if (await _scheduler.CheckExists(jobKey))
                {
                    return false;
                }

                var jobDetail = JobBuilder
                    .Create<FlightPathUpdateJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData(SimulationConstants.Quartz.UAV_ID, uavId)
                    .Build();

                var trigger = TriggerBuilder
                    .Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithSimpleSchedule(x =>
                        x.WithIntervalInSeconds(intervalSeconds).RepeatForever()
                    )
                    .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUAVFlightPathJob(int uavId)
        {
            try
            {
                var jobKey = new JobKey($"{SimulationConstants.Quartz.IDENTITY_PREFIX}{uavId}");

                var deleted = await _scheduler.DeleteJob(jobKey);

                return deleted;
            }
            catch (Exception ex)
            {
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
                    return false;
                }

                await _scheduler.PauseJob(jobKey);
                return true;
            }
            catch (Exception ex)
            {
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
                    return false;
                }

                await _scheduler.ResumeJob(jobKey);
                return true;
            }
            catch (Exception ex)
            {
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
                    var jobKeys = await _scheduler.GetJobKeys(
                        GroupMatcher<JobKey>.GroupEquals(group)
                    );
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
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task StartScheduler()
        {
            try
            {
                if (_scheduler.IsShutdown)
                {
                    return;
                }

                if (!_scheduler.IsStarted)
                {
                    await _scheduler.Start();
                }
            }
            catch (Exception ex)
            {
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
                    var jobKeys = await _scheduler.GetJobKeys(
                        GroupMatcher<JobKey>.GroupEquals(group)
                    );
                    foreach (
                        var jobKey in jobKeys.Where(jk =>
                            jk.Name.StartsWith(SimulationConstants.Quartz.IDENTITY_PREFIX)
                        )
                    )
                    {
                        var deleted = await _scheduler.DeleteJob(jobKey);
                        if (deleted)
                            deletedCount++;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
