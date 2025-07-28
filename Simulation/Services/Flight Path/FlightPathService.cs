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

        var t = uav.TelemetryData;
        _currentLocation = new Location(
            t.GetValueOrDefault(TelemetryFields.LocationLatitude),
            t.GetValueOrDefault(TelemetryFields.LocationLongitude),
            t.GetValueOrDefault(TelemetryFields.LocationAltitudeAmsl));
        _currentSpeedKmph = t.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, 0.0);
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
        var t = _uav.TelemetryData;
        double remKm = FlightPathMathHelper.CalculateDistance(_currentLocation, _destination) / 1000.0;

        var phaseDetails = FlightPhaseFactory.DeterminePhaseDetails(
            _currentLocation,
            _destination,
            _cruiseAltitude);

        _currentSpeedKmph = _speedController.ComputeNextSpeed(
            _currentSpeedKmph,
            remKm,
            _uav.MaxAcceleration,
            _uav.MaxDeceleration,
            DeltaSeconds,
            _uav.MaxCruiseSpeedKmph);
        t[TelemetryFields.LocationGroundSpeed] = _currentSpeedKmph;

        double dtHrs = DeltaSeconds / 3600.0;
        var next = _motionCalculator.CalculateNext(
            _currentLocation,
            _destination,
            _currentSpeedKmph,
            dtHrs,
            phaseDetails.PitchDegrees,
            phaseDetails.TargetAltitude);

        var (yaw, _, roll) = _orientationCalculator.ComputeOrientation(
            _currentLocation,
            next,
            _currentSpeedKmph,
            DeltaSeconds);

        _currentLocation = next;
        UpdateTelemetry(t, next, _currentSpeedKmph, yaw, phaseDetails.PitchDegrees, roll);

        double remM = remKm * 1000.0;
        _logger.LogInformation(
            "[{Time:HH:mm:ss}] Phase {Phase} | Lat {Lat:F6}° | Lon {Lon:F6}° | Alt {Alt:F1}m | Spd {Spd:F1}km/h | Pitch {Pitch:F1}° | Roll {Roll:F1}° | Rem {Rem:F1}m",
            DateTime.Now,
            phaseDetails.Phase,
            next.Latitude,
            next.Longitude,
            next.Altitude,
            _currentSpeedKmph,
            phaseDetails.PitchDegrees,
            roll,
            remM);

        if (remM <= SimulationConstants.FlightPath.LOCATION_PRECISION)
        {
            _logger.LogInformation(
                "MISSION COMPLETED at Lat {Lat:F6}, Lon {Lon:F6}, Alt {Alt:F1}",
                next.Latitude,
                next.Longitude,
                next.Altitude);
            StopTracking();
            MissionCompleted?.Invoke();
        }
        else
        {
            LocationUpdated?.Invoke(next);
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