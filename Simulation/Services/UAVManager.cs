
using Quartz;
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using System.Collections.Concurrent;
using Simulation.Common.constants;

namespace Simulation.Services
{
    public class UAVManager
    {
        private readonly ConcurrentDictionary<int, UAVMissionContext?> _uavs;
        private readonly IMotionCalculator _motionCalculator;
        private readonly ISpeedController _speedController;
        private readonly IOrientationCalculator _orientationCalculator;
        private readonly ILogger<FlightPathService> _logger;
        private readonly IScheduler _scheduler;

        public UAVManager(
            IMotionCalculator motionCalculator,
            ISpeedController speedController,
            IOrientationCalculator orientationCalculator,
            ILogger<FlightPathService> logger,
            IScheduler scheduler)
        {
            _uavs = new ConcurrentDictionary<int, UAVMissionContext?>();
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
            _scheduler = scheduler;
        }
    

        public void AddUAV(UAV uav)
        {
            var flightService = new FlightPathService(
                _motionCalculator,
                _speedController,
                _orientationCalculator,
                _logger
            );
            
            _uavs.TryAdd(uav.TailId,new UAVMissionContext(uav,flightService));
        }

        public void RemoveUAV(int tailId)
        {
            if (_uavs.TryRemove(tailId, out var uavData))
            {
                uavData.Service.Dispose();
            }
        }

        public UAVMissionContext? GetUAVContext(int tailId)
        {
            return _uavs.GetValueOrDefault(tailId) ?? null;
        }

        public async Task<bool> StartMission(UAV uav, Location destination, string missionId)
        {
            uav.CurrentMissionId = missionId;
            if (!_uavs.ContainsKey(uav.TailId))
            {
                AddUAV(uav);
            }

            UAVMissionContext context = _uavs[uav.TailId];
            context.Service.Initialize(uav, destination);
            context.Service.StartFlightPath();

            var jobDetail = JobBuilder.Create<FlightPathUpdateJob>()
                .WithIdentity($"FlightPath-{uav.TailId}")
                .UsingJobData("UAVId", uav.TailId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"FlightPathTrigger-{uav.TailId}")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds((int)SimulationConstants.FlightPath.DELTA_SECONDS)
                    .RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);

            context.Service.MissionCompleted += async () =>
            {
                await _scheduler.DeleteJob(jobDetail.Key);
                uav.CurrentMissionId = string.Empty;
                context.Service.Dispose();
                _uavs.TryRemove(uav.TailId, out _);
            };


            return true; 
        }

        public bool SwitchDestination(int tailId, Location newDestination)
        {
            if (_uavs.TryGetValue(tailId, out var uavData))
            {
                uavData.Service.SwitchDestination(newDestination);
                return true;
            }
            return false;
        }

        public int ActiveUAVCount => _uavs.Count;
        
        public IEnumerable<int> GetActiveTailIds => _uavs.Keys;
    }
}
