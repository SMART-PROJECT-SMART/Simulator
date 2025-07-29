using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.helpers;

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
    private bool _isDisposed;
    private bool _isRunning;
    private bool _missionCompleted;
    private bool _timerDisposed;
    private Location _previousLocation;

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

        var t = _uav.TelemetryData;
        _uav.TelemetryData.TryGetValue(TelemetryFields.Latitude, out double lat);
        _uav.TelemetryData.TryGetValue(TelemetryFields.Longitude, out double lon);
        _uav.TelemetryData.TryGetValue(TelemetryFields.Altitude, out double alt);
        _uav.TelemetryData.TryGetValue(TelemetryFields.CurrentSpeedKmph, out double spd);

        _uav.TelemetryData[TelemetryFields.Latitude] = lat;
        _uav.TelemetryData[TelemetryFields.Longitude] = lon;
        _uav.TelemetryData[TelemetryFields.Altitude] = alt;
        _uav.TelemetryData[TelemetryFields.CurrentSpeedKmph] = Math.Max(
            SimulationConstants.FlightPath.MIN_SPEED_KMH,
            spd);

        _previousLocation = new Location(lat, lon, alt);

        _logger.LogInformation(
            "Starting position ({Lat:F6}, {Lon:F6}, {Alt:F1}), Speed {Spd:F1} km/h",
            lat, lon, alt, spd);
    }

    public void StartFlightPath()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(FlightPathService));
        if (_isRunning) return;

        _isRunning = true;
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(SimulationConstants.FlightPath.DELTA_SECONDS));
    }

    private void UpdateLocation(object? state)
    {
        var telemetry = _uav.TelemetryData;
        var currentLoc = new Location(
            telemetry[TelemetryFields.Latitude],
            telemetry[TelemetryFields.Longitude],
            telemetry[TelemetryFields.Altitude]);
        double remainingMeters = FlightPathMathHelper.CalculateDistance(currentLoc, _destination);

        if (remainingMeters <= SimulationConstants.FlightPath.Location_PRECISION_M &&
            Math.Abs(currentLoc.Altitude - _destination.Altitude) <= SimulationConstants.FlightPath.ALTITUDE_PRECISION_M)
        {
            CompleteMission(currentLoc);
            return;
        }

        double pitchDeg;
        if (currentLoc.Altitude + SimulationConstants.FlightPath.ALTITUDE_PRECISION_M < _cruiseAltitude)
            pitchDeg = SimulationConstants.FlightPath.MAX_CLIMB_DEG;
        else if (currentLoc.Altitude - SimulationConstants.FlightPath.ALTITUDE_PRECISION_M > _destination.Altitude)
            pitchDeg = -SimulationConstants.FlightPath.MAX_DESCENT_DEG;
        else
            pitchDeg = 0;

        telemetry[TelemetryFields.PitchDeg] = pitchDeg;

        double newSpeed = _speedController.ComputeNextSpeed(
            telemetry,
            remainingMeters/1000,
            SimulationConstants.FlightPath.DELTA_SECONDS);
        telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

        var nextLocRaw = _motionCalculator.CalculateNext(
            telemetry,
            currentLoc,
            _destination,
            SimulationConstants.FlightPath.DELTA_SECONDS);

        double maxVert = SimulationConstants.FlightPath.MAX_CLIMB_RATE_MPS
                       * SimulationConstants.FlightPath.DELTA_SECONDS;
        double vertDelta = nextLocRaw.Altitude - currentLoc.Altitude;
        vertDelta = Math.Clamp(vertDelta, -maxVert, maxVert);
        var nextLoc = new Location(
            nextLocRaw.Latitude,
            nextLocRaw.Longitude,
            currentLoc.Altitude + vertDelta);

        AxisDegrees axis = _orientationCalculator.ComputeOrientation(
            telemetry,
            _previousLocation,
            currentLoc,
            _destination,
            SimulationConstants.FlightPath.DELTA_SECONDS);
        axis = new AxisDegrees(axis.Yaw, axis.Pitch, axis.Roll);

        telemetry[TelemetryFields.Latitude] = nextLoc.Latitude;
        telemetry[TelemetryFields.Longitude] = nextLoc.Longitude;
        telemetry[TelemetryFields.Altitude] = nextLoc.Altitude;
        telemetry[TelemetryFields.YawDeg] = axis.Yaw;
        telemetry[TelemetryFields.PitchDeg] = axis.Pitch;
        telemetry[TelemetryFields.RollDeg] = axis.Roll;

        _previousLocation = currentLoc;

        _logger.LogInformation(
            "UAV {UavId} | Lat {Lat:F6} | Lon {Lon:F6} | Alt {Alt:F1}m | Spd {Spd:F1}km/h | Yaw {Yaw:F1}° | Pitch {Pitch:F1}° | Roll {Roll:F1}° | Rem {Rem:F3}m",
            _uav.TailId,
            nextLoc.Latitude,
            nextLoc.Longitude,
            nextLoc.Altitude,
            newSpeed,
            axis.Yaw,
            axis.Pitch,
            axis.Roll,
            remainingMeters);

        LocationUpdated?.Invoke(nextLoc);
    }




    private void CompleteMission(Location loc)
    {
        _missionCompleted = true;
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        _timer.Dispose();
        _timerDisposed = true;

        _logger.LogInformation(
            "MISSION COMPLETED at ({Lat:F6},{Lon:F6},{Alt:F1}), rem=0",
            loc.Latitude, loc.Longitude, loc.Altitude);
        MissionCompleted?.Invoke();
    }


    public void Dispose()
    {
        if (_timerDisposed) return;
        _timer.Dispose();
        _timerDisposed = true;
        _isDisposed = true;
    }
}
