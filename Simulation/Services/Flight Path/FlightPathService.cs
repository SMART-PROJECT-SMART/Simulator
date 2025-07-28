using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Factories.Flight_Phase;
using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;

namespace Simulation.Services.Flight_Path;

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
    private bool _isDisposed;
    private bool _isRunning;
    private bool _missionCompleted;
    private bool _timerDisposed;

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

        InitializeFromUAV();
    }

    private void InitializeFromUAV()
    {
        var telemetry = _uav.TelemetryData ?? new Dictionary<TelemetryFields, double>();
        
        _currentLocation = new Location(
            telemetry.GetValueOrDefault(TelemetryFields.LocationLatitude, 0.0),
            telemetry.GetValueOrDefault(TelemetryFields.LocationLongitude, 0.0),
            telemetry.GetValueOrDefault(TelemetryFields.LocationAltitudeAmsl, 0.0));
            
        _currentSpeedKmph = Math.Max(SimulationConstants.FlightPath.MIN_SPEED_KMH, telemetry.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, SimulationConstants.FlightPath.MIN_SPEED_KMH));
    }

    public void StartFlightPath()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(FlightPathService));

        if (_isRunning)
        {
            _logger.LogWarning("Flight path is already running for UAV {UavId}", _uav.Id);
            return;
        }

        _logger.LogInformation("Starting flight for UAV {UavId} from ({Lat:F6}, {Lon:F6}) to ({DestLat:F6}, {DestLon:F6})", 
            _uav.Id, _currentLocation.Latitude, _currentLocation.Longitude, _destination.Latitude, _destination.Longitude);
        
        _isRunning = true;
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(SimulationConstants.FlightPath.DELTA_SECONDS));
    }

    public void StopTracking()
    {
        if (_isDisposed) return;

        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        _isRunning = false;
    }

    private void UpdateLocation(object? _)
    {
        if (_isDisposed || !_isRunning || _missionCompleted || _timerDisposed) return;

        var telemetry = _uav.TelemetryData ?? new Dictionary<TelemetryFields, double>();
        double remainingKm = FlightPathMathHelper.CalculateDistance(_currentLocation, _destination) / 1000.0;

        var phaseDetails = FlightPhaseFactory.DeterminePhaseDetails(
            _currentLocation,
            _destination,
            _cruiseAltitude);

        _currentSpeedKmph = _speedController.ComputeNextSpeed(
            _currentSpeedKmph,
            remainingKm,
            _uav.MaxAcceleration,
            _uav.MaxDeceleration,
            SimulationConstants.FlightPath.DELTA_SECONDS,
            _uav.MaxCruiseSpeedKmph);

        telemetry[TelemetryFields.LocationGroundSpeed] = _currentSpeedKmph;

        double deltaHours = SimulationConstants.FlightPath.DELTA_SECONDS / 3600.0;
        var nextLocation = _motionCalculator.CalculateNext(
            _currentLocation,
            _destination,
            _currentSpeedKmph,
            deltaHours,
            phaseDetails.PitchDegrees,
            phaseDetails.TargetAltitude);

        var (yaw, pitch, roll) = _orientationCalculator.ComputeOrientation(
            _currentLocation,
            nextLocation,
            _currentSpeedKmph,
            SimulationConstants.FlightPath.DELTA_SECONDS);

        _currentLocation = nextLocation;
        UpdateTelemetry(telemetry, nextLocation, _currentSpeedKmph, yaw, pitch, roll);

        if (remainingKm <= 0.0 || remainingKm <= SimulationConstants.FlightPath.LOCATION_PRECISION_KM)
        {
            _logger.LogInformation("Mission completion triggered: remainingKm={RemainingKm:F6}, precision={Precision}", 
                remainingKm, SimulationConstants.FlightPath.LOCATION_PRECISION_KM);
            CompleteMission(nextLocation);
        }
        else
        {
            LogProgress(phaseDetails, nextLocation, remainingKm);
            LocationUpdated?.Invoke(nextLocation);
        }
    }

    private void LogProgress(PhaseDetails phaseDetails, Location location, double remainingKm)
    {
        _logger.LogInformation(
            "[{Time:HH:mm:ss}] UAV {UavId} | Phase {Phase} | Lat {Lat:F6}° | Lon {Lon:F6}° | Alt {Alt:F1}m | Spd {Spd:F1}km/h | Pitch {Pitch:F1}° | Rem {Rem:F3}km",
            DateTime.Now,
            _uav.Id,
            phaseDetails.Phase,
            location.Latitude,
            location.Longitude,
            location.Altitude,
            _currentSpeedKmph,
            phaseDetails.PitchDegrees,
            remainingKm);
    }

    private void CompleteMission(Location finalLocation)
    {
        _missionCompleted = true;
        _isRunning = false;
        
        if (!_timerDisposed)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timerDisposed = true;
        }
        
        double finalDistanceKm = FlightPathMathHelper.CalculateDistance(finalLocation, _destination) / 1000.0;
        
        _logger.LogInformation(
            "MISSION COMPLETED for UAV {UavId} at Lat {Lat:F6}, Lon {Lon:F6}, Alt {Alt:F1}, Final Distance: {Distance:F6}km",
            _uav.Id,
            finalLocation.Latitude,
            finalLocation.Longitude,
            finalLocation.Altitude,
            finalDistanceKm);
            
        MissionCompleted?.Invoke();
    }

    private static void UpdateTelemetry(
        Dictionary<TelemetryFields, double> telemetry,
        Location location,
        double speed,
        double yaw,
        double pitch,
        double roll)
    {
        telemetry[TelemetryFields.LocationLatitude] = location.Latitude;
        telemetry[TelemetryFields.LocationLongitude] = location.Longitude;
        telemetry[TelemetryFields.LocationAltitudeAmsl] = location.Altitude;
        telemetry[TelemetryFields.LocationGroundSpeed] = speed;
        telemetry[TelemetryFields.LocationYaw] = yaw;
        telemetry[TelemetryFields.LocationPitch] = pitch;
        telemetry[TelemetryFields.LocationRoll] = roll;
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        StopTracking();
        
        if (!_timerDisposed)
        {
            _timer?.Dispose();
            _timerDisposed = true;
        }
        
        _isDisposed = true;
    }
}