using Core.Models;
﻿using Core.Common.Enums;
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
    private UAV _uav;
    private Location _destination;
    private double _cruiseAltitude;
    private readonly IMotionCalculator _motionCalculator;
    private readonly ISpeedController _speedController;
    private readonly IOrientationCalculator _orientationCalculator;

    private bool _isRunning;
    private bool _missionCompleted;
    private Location _previousLocation;
    private Location _startLocation;

    public event Action<Location>? LocationUpdated;
    public event Action? MissionCompleted;
    public event Action<Dictionary<TelemetryFields, double>>? TelemetryUpdated;

    public FlightPathService(
        IMotionCalculator motionCalculator,
        ISpeedController speedController,
        IOrientationCalculator orientationCalculator
    )
    {
        _motionCalculator = motionCalculator;
        _speedController = speedController;
        _orientationCalculator = orientationCalculator;
    }

    public void Initialize(UAV uav, Location destination)
    {
        _uav = uav;
        _destination = destination;
        _cruiseAltitude = _uav.TelemetryData[TelemetryFields.CruiseAltitude];
        _uav.TelemetryData[TelemetryFields.TailId] = (double)_uav.TailId;

        var t = _uav.TelemetryData;
        t.TryGetValue(TelemetryFields.Latitude, out double lat);
        t.TryGetValue(TelemetryFields.Longitude, out double lon);
        t.TryGetValue(TelemetryFields.Altitude, out double alt);
        t.TryGetValue(TelemetryFields.CurrentSpeedKmph, out double spd);
        t[TelemetryFields.CurrentSpeedKmph] = Math.Max(
            SimulationConstants.FlightPath.MIN_SPEED_KMH,
            spd
        );
        _previousLocation = new Location(lat, lon, alt);
    }

    public void StartFlightPath()
    {
        if (_isRunning)
            return;

        _startLocation = _uav.GetLocation();
        _isRunning = true;
        _uav.TakeOff();
    }

    public void SwitchDestination(Location newDestination)
    {
        _destination = newDestination;
    }

    public Location GetDestination() => _destination;

    public void UpdateLocation()
    {
        if (_missionCompleted)
            return;

        var telemetry = _uav.TelemetryData;
        var currentLoc = new Location(
            telemetry[TelemetryFields.Latitude],
            telemetry[TelemetryFields.Longitude],
            telemetry[TelemetryFields.Altitude]
        );
        double remainingMeters = FlightPathMathHelper.CalculateDistance(currentLoc, _destination);
        double distanceFromStart = FlightPathMathHelper.CalculateDistance(
            _startLocation,
            currentLoc
        );


        if (remainingMeters <SimulationConstants.FlightPath.MISSION_COMPLETION_RADIUS_M)
        {
            _uav.Land();
            _missionCompleted = true;
            MissionCompleted?.Invoke();
            return;
        }

        double newSpeed = _speedController.ComputeNextSpeed(
            telemetry,
            _uav.Properties[UAVProperties.MaxAcceleration],
            _uav.Properties[UAVProperties.MaxCruiseSpeed],
            remainingMeters,
            SimulationConstants.FlightPath.DELTA_SECONDS,
            _uav.Properties[UAVProperties.MaxAcceleration],
            _uav.Properties[UAVProperties.MaxAcceleration],
            _uav.Properties[UAVProperties.MaxAcceleration],
            _uav.Properties[UAVProperties.MaxCruiseSpeed]
        );
        telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

        double maxCruise = _uav.Properties[UAVProperties.MaxAcceleration];
        double throttlePct = Math.Clamp(newSpeed / maxCruise * 100.0, 0.0, 100.0);
        telemetry[TelemetryFields.ThrottlePercent] = throttlePct;

        _uav.ConsumeFuel(SimulationConstants.FlightPath.DELTA_SECONDS);
        telemetry[TelemetryFields.SignalStrength] =
            FlightPhysicsCalculator.CalculateReceivedSignalStrengthDbm(
                _uav.Properties[UAVProperties.TransmitPower],
                _uav.Properties[UAVProperties.TransmitAntennaGain],
                _uav.Properties[UAVProperties.ReceiveAntennaGain],
                _uav.Properties[UAVProperties.TransmitLoss],
                _uav.Properties[UAVProperties.ReceiveLoss],
                _uav.Properties[UAVProperties.Frequency],
                distanceFromStart
            );

        if (MissionAborted(telemetry))
        {
            _missionCompleted = true;
            MissionCompleted?.Invoke();
            return;
        }

        var axis = _orientationCalculator.ComputeOrientation(
            telemetry,
            _previousLocation,
            currentLoc,
            _destination,
            SimulationConstants.FlightPath.DELTA_SECONDS
        );
        telemetry[TelemetryFields.PitchDeg] = axis.Pitch;

        var nextLoc = _motionCalculator.CalculateNext(
            telemetry,
            currentLoc,
            _destination,
            SimulationConstants.FlightPath.DELTA_SECONDS,
            _uav.Properties[UAVProperties.MaxAcceleration],
            _uav.Properties[UAVProperties.MaxAcceleration]
        );
        telemetry[TelemetryFields.Latitude] = nextLoc.Latitude;
        telemetry[TelemetryFields.Longitude] = nextLoc.Longitude;
        telemetry[TelemetryFields.Altitude] = nextLoc.Altitude;
        telemetry[TelemetryFields.YawDeg] = axis.Yaw;
        telemetry[TelemetryFields.RollDeg] = axis.Roll;

        _previousLocation = currentLoc;
        telemetry[TelemetryFields.FlightTimeSec] += SimulationConstants.FlightPath.DELTA_SECONDS;
        _uav.UpdateRpm();

        TelemetryUpdated?.Invoke(telemetry);
        LocationUpdated?.Invoke(nextLoc);
    }

    public bool MissionAborted(Dictionary<TelemetryFields, double> telemetryData)
    {
        return telemetryData[TelemetryFields.FuelAmount] <= 0.0
            || telemetryData[TelemetryFields.SignalStrength]
                < SimulationConstants.TelemetryData.NO_SIGNAL
            || telemetryData[TelemetryFields.EngineDegrees]
                > SimulationConstants.FlightPath.OVERHEAT;
    }

    private string DetermineAbortReason(Dictionary<TelemetryFields, double> telemetryData)
    {
        if (telemetryData[TelemetryFields.FuelAmount] <= 0.0)
        {
            return SimulationConstants.FlightPath.ABORT_REASON_FUEL_DEPLETION;
        }

        if (
            telemetryData[TelemetryFields.SignalStrength]
            < SimulationConstants.TelemetryData.NO_SIGNAL
        )
        {
            return SimulationConstants.FlightPath.ABORT_REASON_COMMUNICATION_LOSS;
        }

        if (telemetryData[TelemetryFields.EngineDegrees] > SimulationConstants.FlightPath.OVERHEAT)
        {
            return SimulationConstants.FlightPath.ABORT_REASON_ENGINE_OVERHEAT;
        }

        return SimulationConstants.FlightPath.ABORT_REASON_UNKNOWN;
    }

    public void Dispose()
    {
        _isRunning = false;
    }
}
