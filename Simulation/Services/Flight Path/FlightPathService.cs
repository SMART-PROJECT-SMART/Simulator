using System;
using System.Threading;
using Microsoft.Extensions.Logging;
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
    private bool _isRunning;
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
        ILogger<FlightPathService> logger
    )
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
        t.TryGetValue(TelemetryFields.Latitude, out double lat);
        t.TryGetValue(TelemetryFields.Longitude, out double lon);
        t.TryGetValue(TelemetryFields.Altitude, out double alt);
        t.TryGetValue(TelemetryFields.CurrentSpeedKmph, out double spd);
        t[TelemetryFields.CurrentSpeedKmph] = Math.Max(SimulationConstants.FlightPath.MIN_SPEED_KMH, spd);
        _previousLocation = new Location(lat, lon, alt);
        _logger.LogInformation("Starting position ({Lat:F6}, {Lon:F6}, {Alt:F1}), Speed {Spd:F1} km/h", lat, lon, alt, spd);
    }

    public void StartFlightPath()
    {
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
            telemetry[TelemetryFields.Altitude]
        );
        double remainingMeters = FlightPathMathHelper.CalculateDistance(currentLoc, _destination);

        if (remainingMeters <= SimulationConstants.FlightPath.LOCATION_PRECISION_M
            && Math.Abs(currentLoc.Altitude - _destination.Altitude) <= SimulationConstants.FlightPath.ALTITUDE_PRECISION_M)
        {
            CompleteMission(currentLoc, true);
            return;
        }

        double newSpeed = _speedController.ComputeNextSpeed(
            telemetry, remainingMeters, SimulationConstants.FlightPath.DELTA_SECONDS
        );
        telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

        double maxCruise = telemetry.GetValueOrDefault(TelemetryFields.MaxCruiseSpeedKmph, 1.0);
        double throttlePct = Math.Clamp(newSpeed / maxCruise * 100.0, 0.0, 100.0);
        telemetry[TelemetryFields.ThrottlePercent] = throttlePct;

        double thrustMax = telemetry.GetValueOrDefault(TelemetryFields.ThrustAfterInfluence, 0.0);
        double thrust = thrustMax * (throttlePct / 100.0);
        double sfc = _uav.FuelConsumption;
        double burnKg = thrust * sfc * SimulationConstants.FlightPath.DELTA_SECONDS;
        double fuelLeft = Math.Max(telemetry.GetValueOrDefault(TelemetryFields.FuelAmount, 0.0) - burnKg, 0.0);
        telemetry[TelemetryFields.FuelAmount] = fuelLeft;

        _logger.LogDebug(
            "Fuel burn debug | Throttle {Throttle:F1}% | Thrust {Thrust:F1} N | SFC {Sfc:E4} kg/(N·s) | Burn {Burn:F4} kg | Remaining {Fuel:F3} kg",
            throttlePct, thrust, sfc, burnKg, fuelLeft
        );

        if (fuelLeft <= 0.0)
        {
            _logger.LogInformation("MISSION ABORTED: UAV {UavId} ran out of fuel at ({Lat:F6},{Lon:F6},{Alt:F1})",
                _uav.TailId, currentLoc.Latitude, currentLoc.Longitude, currentLoc.Altitude);
            CompleteMission(currentLoc, false);
            return;
        }

        double pitchDeg = currentLoc.Altitude + SimulationConstants.FlightPath.ALTITUDE_PRECISION_M < _cruiseAltitude
            ? SimulationConstants.FlightPath.MAX_CLIMB_DEG
            : currentLoc.Altitude - SimulationConstants.FlightPath.ALTITUDE_PRECISION_M > _destination.Altitude
                ? -SimulationConstants.FlightPath.MAX_DESCENT_DEG
                : 0.0;
        telemetry[TelemetryFields.PitchDeg] = pitchDeg;

        var nextLocRaw = _motionCalculator.CalculateNext(
            telemetry, currentLoc, _destination, SimulationConstants.FlightPath.DELTA_SECONDS
        );
        double maxVert = SimulationConstants.FlightPath.MAX_CLIMB_RATE_MPS * SimulationConstants.FlightPath.DELTA_SECONDS;
        double vertDelta = Math.Clamp(nextLocRaw.Altitude - currentLoc.Altitude, -maxVert, maxVert);
        var nextLoc = new Location(nextLocRaw.Latitude, nextLocRaw.Longitude, currentLoc.Altitude + vertDelta);

        var axis = _orientationCalculator.ComputeOrientation(
            telemetry, _previousLocation, currentLoc, _destination, SimulationConstants.FlightPath.DELTA_SECONDS
        );
        telemetry[TelemetryFields.Latitude] = nextLoc.Latitude;
        telemetry[TelemetryFields.Longitude] = nextLoc.Longitude;
        telemetry[TelemetryFields.Altitude] = nextLoc.Altitude;
        telemetry[TelemetryFields.YawDeg] = axis.Yaw;
        telemetry[TelemetryFields.PitchDeg] = axis.Pitch;
        telemetry[TelemetryFields.RollDeg] = axis.Roll;
        _previousLocation = currentLoc;

        _logger.LogInformation(
            "UAV {UavId} | Lat {Lat:F6} | Lon {Lon:F6} | Alt {Alt:F1}m | Spd {Spd:F1}km/h | Yaw {Yaw:F1}° | Pitch {Pitch:F1}° | Roll {Roll:F1}° | Rem {Rem:F1}m | Fuel {Fuel:F3}kg",
            _uav.TailId,
            nextLoc.Latitude,
            nextLoc.Longitude,
            nextLoc.Altitude,
            newSpeed,
            axis.Yaw,
            axis.Pitch,
            axis.Roll,
            remainingMeters,
            fuelLeft
        );

        LocationUpdated?.Invoke(nextLoc);
    }

    private void CompleteMission(Location loc, bool wasCompleted)
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        _timer.Dispose();
        _timerDisposed = true;
        string status = wasCompleted ? "COMPLETED" : "ABORTED";
        _logger.LogInformation("MISSION {Status} at ({Lat:F6},{Lon:F6},{Alt:F1}), rem=0",
            status, loc.Latitude, loc.Longitude, loc.Altitude);
        MissionCompleted?.Invoke();
    }

    public void Dispose()
    {
        if (_timerDisposed) return;
        _timer.Dispose();
        _timerDisposed = true;
        _isRunning = false;
    }
}
