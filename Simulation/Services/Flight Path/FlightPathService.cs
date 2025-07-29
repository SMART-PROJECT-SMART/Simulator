using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Models.UAVs;
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

        var t = _uav.TelemetryData;
        _uav.TelemetryData.TryGetValue(TelemetryFields.Latitude, out double lat);
        _uav.TelemetryData.TryGetValue(TelemetryFields.longitude, out double lon);
        _uav.TelemetryData.TryGetValue(TelemetryFields.Altitude, out double alt);
        _uav.TelemetryData.TryGetValue(TelemetryFields.CurrentSpeedKmph, out double spd);

        _uav.TelemetryData[TelemetryFields.Latitude] = lat;
        _uav.TelemetryData[TelemetryFields.longitude] = lon;
        _uav.TelemetryData[TelemetryFields.Altitude] = alt;
        _uav.TelemetryData[TelemetryFields.CurrentSpeedKmph] = Math.Max(
            SimulationConstants.FlightPath.MIN_SPEED_MPS,
            spd);

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
        if (_isDisposed || !_isRunning || _missionCompleted || _timerDisposed) return;

        var telemetry = _uav.TelemetryData;

        var currentLoc = new Location(
            telemetry[TelemetryFields.Latitude],
            telemetry[TelemetryFields.longitude],
            telemetry[TelemetryFields.Altitude]);
        double remainingKm =
            FlightPathMathHelper.CalculateDistance(currentLoc, _destination) / 1000.0;

        if (remainingKm <= SimulationConstants.FlightPath.LOCATION_PRECISION_KM
            && Math.Abs(currentLoc.Altitude - _destination.Altitude) <= SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            CompleteMission(currentLoc);
            return;
        }

        double newSpeed = _speedController.ComputeNextSpeed(
            telemetry,
            remainingKm,
            SimulationConstants.FlightPath.DELTA_SECONDS);
        telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

        var nextLoc = _motionCalculator.CalculateNext(
            telemetry,
            currentLoc,
            _destination,
            SimulationConstants.FlightPath.DELTA_SECONDS);

        AxisDegrees axisDegrees = _orientationCalculator.ComputeOrientation(
            telemetry,
            currentLoc,
            nextLoc,
            SimulationConstants.FlightPath.DELTA_SECONDS);

        telemetry[TelemetryFields.Latitude] = nextLoc.Latitude;
        telemetry[TelemetryFields.longitude] = nextLoc.Longitude;
        telemetry[TelemetryFields.Altitude] = nextLoc.Altitude;
        telemetry[TelemetryFields.YawDeg] = axisDegrees.Yaw;
        telemetry[TelemetryFields.PitchDeg] = axisDegrees.Pitch;
        telemetry[TelemetryFields.RollDeg] = axisDegrees.Roll;

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
