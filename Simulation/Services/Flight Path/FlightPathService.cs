using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Factories.Flight_Phase;
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using System;
using System.Collections.Generic;
using System.Threading;

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
    private enum ApproachPattern { Normal, ExtendedFinal, CirclingApproach }
    private ApproachPattern _currentApproachPattern;

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
        _currentApproachPattern = ApproachPattern.Normal;
        _timer = new Timer(UpdateLocation, null, Timeout.Infinite, Timeout.Infinite);

        var telemetry = _uav.TelemetryData ?? new Dictionary<TelemetryFields, double>();
        _currentLocation = new Location(
            telemetry.GetValueOrDefault(TelemetryFields.LocationLatitude, 0.0),
            telemetry.GetValueOrDefault(TelemetryFields.LocationLongitude, 0.0),
            telemetry.GetValueOrDefault(TelemetryFields.LocationAltitudeAmsl, 0.0));
        _currentSpeedKmph = Math.Max(
            SimulationConstants.FlightPath.MIN_SPEED_KMH,
            telemetry.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, SimulationConstants.FlightPath.MIN_SPEED_KMH));
    }

    public void StartFlightPath()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(FlightPathService));
        if (_isRunning) return;
        _logger.LogInformation(
            "Starting flight for UAV {UavId} from ({Lat:F6}, {Lon:F6}) to ({DestLat:F6}, {DestLon:F6})",
            _uav.TailId,
            _currentLocation.Latitude,
            _currentLocation.Longitude,
            _destination.Latitude,
            _destination.Longitude);
        _isRunning = true;
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(SimulationConstants.FlightPath.DELTA_SECONDS));
    }

    private void UpdateLocation(object? _)
    {
        if (_isDisposed || !_isRunning || _missionCompleted || _timerDisposed) return;

        var telemetry = _uav.TelemetryData ?? new Dictionary<TelemetryFields, double>();
        double remainingKm = FlightPathMathHelper.CalculateDistance(_currentLocation, _destination) / 1000.0;

        if (remainingKm <= SimulationConstants.FlightPath.LOCATION_PRECISION_KM &&
            Math.Abs(_currentLocation.Altitude - _destination.Altitude) <= SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            CompleteMission(_currentLocation);
            return;
        }

        bool veryClose = remainingKm <= SimulationConstants.FlightPath.LOCATION_PRECISION_KM * 0.5;
        bool slowEnough = _currentSpeedKmph <= 8.0;

        if (veryClose && _currentApproachPattern == ApproachPattern.Normal)
        {
            _currentSpeedKmph = 5.0;
            telemetry[TelemetryFields.LocationGroundSpeed] = _currentSpeedKmph;
        }

        if (veryClose && slowEnough && _currentApproachPattern == ApproachPattern.Normal)
        {
            var finalLoc = new Location(_destination.Latitude, _destination.Longitude, _destination.Altitude);
            telemetry[TelemetryFields.LocationGroundSpeed] = 2.0;
            UpdateTelemetry(telemetry, finalLoc, 2.0, 0.0, 0.0, 0.0);
            CompleteMission(finalLoc);
            return;
        }

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
            phaseDetails.TargetLocation,
            _currentSpeedKmph,
            deltaHours,
            phaseDetails.PitchDegrees,
            phaseDetails.TargetLocation.Altitude);

        var (yaw, pitch, roll) = _orientationCalculator.ComputeOrientation(
            _currentLocation,
            nextLocation,
            _currentSpeedKmph,
            SimulationConstants.FlightPath.DELTA_SECONDS);

        _currentLocation = nextLocation;
        UpdateTelemetry(telemetry, nextLocation, _currentSpeedKmph, yaw, pitch, roll);

        string time = DateTime.Now.ToString("HH:mm:ss");
        _logger.LogInformation(
            "[{Time}] UAV {UavId} | Phase {Phase} | Pattern {Pattern} | Lat {Lat:F6} | Lon {Lon:F6} | Alt {Alt:F1}m | Spd {Spd:F1}km/h | Pitch {Pitch:F1}° | Rem {Rem:F3}km",
            time,
            _uav.TailId,
            phaseDetails.Phase,
            _currentApproachPattern,
            nextLocation.Latitude,
            nextLocation.Longitude,
            nextLocation.Altitude,
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
            _uav.TailId,
            finalLocation.Latitude,
            finalLocation.Longitude,
            finalLocation.Altitude,
            finalDistanceKm);
        MissionCompleted?.Invoke();
    }

    private static void UpdateTelemetry(
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

    public void Dispose()
    {
        if (_isDisposed) return;
        StopTracking();
        if (!_timerDisposed)
        {
            _timer.Dispose();
            _timerDisposed = true;
        }
        _isDisposed = true;
    }

    private void StopTracking()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        _isRunning = false;
    }
}
