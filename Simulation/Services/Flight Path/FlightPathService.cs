using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Factories.Flight_Phase;
using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path
{
    public class FlightPathService : IDisposable
    {
        private readonly ILogger<FlightPathService> _logger;
        private readonly UAV _uav;
        private readonly Location _destination;
        private readonly double _cruiseAltitude;
        private readonly IMotionCalculator _motionCalculator;
        private readonly ISpeedController _speedController;
        private readonly IOrientationCalculator _orientationCalculator;
        private readonly Timer _timer;

        private Location _currentLocation;
        private double _currentSpeedKmph;
        private const double DeltaSeconds = 1.0;

        public event Action<Location>? LocationUpdated;
        public event Action? MissionCompleted;

        public FlightPathService(
            UAV uav,
            Location destination,
            double cruiseAltitude,
            IMotionCalculator motionCalculator,
            ISpeedController speedController,
            IOrientationCalculator orientationCalculator,
            ILogger<FlightPathService> logger)
        {
            _uav = uav;
            _destination = destination;
            _cruiseAltitude = cruiseAltitude;
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
            _timer = new Timer(UpdateLocation, null, Timeout.Infinite, Timeout.Infinite);

            var telemetry = uav.TelemetryData;
            _currentLocation = new Location(
                telemetry.GetValueOrDefault(TelemetryFields.LocationLatitude),
                telemetry.GetValueOrDefault(TelemetryFields.LocationLongitude),
                telemetry.GetValueOrDefault(TelemetryFields.LocationAltitudeAmsl));
            _currentSpeedKmph = telemetry.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, 0.0);
        }

        public void StartFlightPath()
        {
            _logger.LogInformation("Starting flight for UAV {UavId}", _uav.Id);
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(DeltaSeconds));
        }

        public void StopTracking()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Stopped flight for UAV {UavId}", _uav.Id);
        }

        private void UpdateLocation(object? _)
        {
            var telemetry = _uav.TelemetryData;
            double remainingKm = FlightPathMathHelper.CalculateDistance(_currentLocation, _destination) / 1000.0;

            var phase = FlightPhaseFactory.DeterminePhase(_currentLocation, _destination, _cruiseAltitude);

            double targetAltitude = phase switch
            {
                FlightPhase.Climb => _cruiseAltitude,
                FlightPhase.Cruise => _cruiseAltitude,
                FlightPhase.Descent => _destination.Altitude,
                _ => _currentLocation.Altitude
            };

            double verticalDiff = targetAltitude - _currentLocation.Altitude;
            double horizontalMeters = remainingKm * 1000.0;
            double pitchDegrees = UnitConversionHelper.ToDegrees(Math.Atan2(verticalDiff, horizontalMeters));

            _currentSpeedKmph = _speedController.ComputeNextSpeed(
                _currentSpeedKmph,
                remainingKm,
                _uav.MaxAcceleration,
                _uav.MaxDeceleration,
                DeltaSeconds,
                _uav.MaxCruiseSpeedKmph);
            telemetry[TelemetryFields.LocationGroundSpeed] = _currentSpeedKmph;

            double deltaHours = DeltaSeconds / 3600.0;
            var nextLocation = _motionCalculator.CalculateNext(
                _currentLocation,
                _destination,
                _currentSpeedKmph,
                deltaHours,
                pitchDegrees,
                targetAltitude);

            var (yaw, _, roll) = _orientationCalculator.ComputeOrientation(
                _currentLocation,
                nextLocation,
                _currentSpeedKmph,
                DeltaSeconds);

            _currentLocation = nextLocation;
            UpdateTelemetry(
                telemetry,
                nextLocation,
                _currentSpeedKmph,
                yaw,
                pitchDegrees,
                roll);

            double remainingMeters = remainingKm * 1000.0;
            _logger.LogInformation(
                "[{Time:HH:mm:ss}] Phase {Phase} | Pos {Lat:F6},{Lon:F6},{Alt:F1}m | Spd {Spd:F1}km/h | Pitch {Pitch:F1}° | Roll {Roll:F1}° | Remain {Rem:F1}m",
                DateTime.Now, phase,
                nextLocation.Latitude, nextLocation.Longitude, nextLocation.Altitude,
                _currentSpeedKmph, pitchDegrees, roll, remainingMeters);

            if (remainingMeters <= SimulationConstants.FlightPath.LOCATION_PRECISION)
            {
                _logger.LogInformation("MISSION COMPLETED at {Lat:F6},{Lon:F6}", nextLocation.Latitude, nextLocation.Longitude);
                StopTracking();
                MissionCompleted?.Invoke();
            }
            else
            {
                LocationUpdated?.Invoke(nextLocation);
            }
        }

        private void UpdateTelemetry(
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

        public void Dispose() => _timer.Dispose();
    }
}
