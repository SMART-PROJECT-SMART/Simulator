using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;

namespace Simulation.Services.Flight_Path
{
    public class FlightPathService : IDisposable
    {
        private readonly ILogger<FlightPathService> _logger;
        private readonly UAV _uav;
        private readonly Location _destination;
        private readonly IMotionCalculator _motionCalculator;
        private readonly ISpeedController _speedController;
        private readonly IOrientationCalculator _orientationCalculator;
        private readonly Timer _updateTimer;

        private Location _currentLocation;
        private double _currentSpeedKmph;
        private readonly double _deltaSeconds = 1.0;

        public event Action<Location>? LocationUpdated;
        public event Action? MissionCompleted;

        public FlightPathService(
            UAV uav,
            Location destination,
            IMotionCalculator motionCalculator,
            ISpeedController speedController,
            IOrientationCalculator orientationCalculator,
            ILogger<FlightPathService> logger)
        {
            _uav = uav;
            _destination = destination;
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
            _updateTimer = new Timer(UpdateLocation, null, Timeout.Infinite, Timeout.Infinite);

            var t = _uav.TelemetryData;
            _currentLocation = new Location(
                t.GetValueOrDefault(TelemetryFields.LocationLatitude),
                t.GetValueOrDefault(TelemetryFields.LocationLongitude),
                t.GetValueOrDefault(TelemetryFields.LocationAltitudeAmsl));
            _currentSpeedKmph = t.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, 0.0);
        }

        public void StartFlightPath()
        {
            _logger.LogInformation("Starting flight for UAV {UavId}", _uav.Id);
            _updateTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_deltaSeconds));
        }

        public void StopTracking()
        {
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Stopped flight for UAV {UavId}", _uav.Id);
        }

        private void UpdateLocation(object? state)
        {
            var telemetry = _uav.TelemetryData;
            double remainingKm = FlightPathMathHelper
                .CalculateDistance(_currentLocation, _destination) / 1000.0;

            _currentSpeedKmph = _speedController.ComputeNextSpeed(
                _currentSpeedKmph,
                remainingKm,
                _uav.MaxAcceleration,
                _uav.MaxDeceleration,
                _deltaSeconds,
                _uav.MaxCruiseSpeedKmph);

            telemetry[TelemetryFields.LocationGroundSpeed] = _currentSpeedKmph;

            double deltaHours = _deltaSeconds / 3600.0;
            var nextLocation = _motionCalculator.CalculateNext(
                _currentLocation,
                _destination,
                _currentSpeedKmph,
                deltaHours);

            var (yaw, pitch, roll) = _orientationCalculator.ComputeOrientation(
                _currentLocation,
                nextLocation,
                _currentSpeedKmph,
                _deltaSeconds);

            _currentLocation = nextLocation;

            UpdateAllTelemetry(
                telemetry,
                nextLocation,
                _currentSpeedKmph,
                yaw,
                pitch,
                roll);

            double remainingMeters = remainingKm * 1000.0;
            _logger.LogInformation(
                "[{Time:HH:mm:ss}] UAV {Id} Pos {Lat:F6},{Lon:F6},{Alt:F1}m Spd {Spd:F1}km/h Yaw {Yaw:F1} Pitch {Pitch:F1} Roll {Roll:F1} Remain {Rem:F1}m",
                DateTime.Now,
                _uav.Id,
                nextLocation.Latitude,
                nextLocation.Longitude,
                nextLocation.Altitude,
                _currentSpeedKmph,
                yaw,
                pitch,
                roll,
                remainingMeters);

            if (remainingMeters <= SimulationConstants.FlightPath.LOCATION_PRECISION)
            {
                _logger.LogInformation(
                    "MISSION COMPLETED at {Lat:F8},{Lon:F8}",
                    nextLocation.Latitude,
                    nextLocation.Longitude);
                StopTracking();
                MissionCompleted?.Invoke();
            }
            else
            {
                LocationUpdated?.Invoke(nextLocation);
            }
        }

        private void UpdateAllTelemetry(
            Dictionary<TelemetryFields, double> telemetry,
            Location loc,
            double speed,
            double yaw,
            double pitch,
            double roll)
        {
            telemetry[TelemetryFields.LocationLatitude] = loc.Latitude;
            telemetry[TelemetryFields.LocationLongitude] = loc.Longitude;
            telemetry[TelemetryFields.LocationAltitudeAmsl] = loc.Altitude;
            telemetry[TelemetryFields.LocationGroundSpeed] = speed;
            telemetry[TelemetryFields.LocationYaw] = yaw;
            telemetry[TelemetryFields.LocationPitch] = pitch;
            telemetry[TelemetryFields.LocationRoll] = roll;
        }

        public void Dispose() => _updateTimer.Dispose();
    }
}
