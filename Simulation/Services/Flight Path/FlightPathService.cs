using Microsoft.Extensions.Logging;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Mission;
using Simulation.Models.Uav;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.helpers;
using System;
using System.Threading;

namespace Simulation.Services
{
    public class FlightPathService : IDisposable
    {
        private readonly ILogger<FlightPathService> _logger;
        private readonly UAV _uav;
        private readonly Mission _mission;
        private readonly IMotionCalculator _motionCalculator;
        private readonly Timer _updateTimer;
        private Location _currentLocation;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        public event Action<Location>? LocationUpdated;
        public event Action? MissionCompleted;

        public FlightPathService(
            UAV uav,
            Mission mission,
            IMotionCalculator motionCalculator,
            ILogger<FlightPathService> logger)
        {
            _uav = uav;
            _mission = mission;
            _motionCalculator = motionCalculator;
            _logger = logger;

            _updateTimer = new Timer(UpdateLocation, null, Timeout.Infinite, Timeout.Infinite);
            InitializeCurrentLocation();
        }

        private void InitializeCurrentLocation()
        {
            var telemetry = _uav.TelemetryData;
            _currentLocation = new Location(
                telemetry.GetValueOrDefault(TelemetryFields.LocationLatitude),
                telemetry.GetValueOrDefault(TelemetryFields.LocationLongitude),
                telemetry.GetValueOrDefault(TelemetryFields.LocationAltitudeAmsl)
            );
        }

        public void StartFlightPath()
        {
            _logger.LogInformation("Starting tracking for UAV {UavId}", _uav.Id);
            _logger.LogInformation(
                "Start Pos: Lat={Lat:F6}, Lon={Lon:F6}, Alt={Alt:F2}m",
                _currentLocation.Latitude, _currentLocation.Longitude, _currentLocation.Altitude);
            _logger.LogInformation(
                "Destination: Lat={DestLat:F6}, Lon={DestLon:F6}, Distance={Dist:F1}m",
                _mission.Destination.Latitude,
                _mission.Destination.Longitude,
                FlightPathMathHelper.CalculateDistance(_currentLocation, _mission.Destination));

            _updateTimer.Change(TimeSpan.Zero, _updateInterval);
        }

        public void StopTracking()
        {
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Tracking stopped for UAV {UavId}", _uav.Id);
        }

        private void UpdateLocation(object? state)
        {
            var telemetry = _uav.TelemetryData;
            double speed = telemetry.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, 0.0);
            double pitch = telemetry.GetValueOrDefault(TelemetryFields.LocationPitch, 0.0);

            var nextLocation = _motionCalculator.CalculateNext(
                _currentLocation,
                _mission.Destination,
                speed,
                pitch,
                _updateInterval.TotalSeconds);

            _currentLocation = nextLocation;

            UpdateTelemetryFields(nextLocation.Latitude, nextLocation.Longitude, nextLocation.Altitude);

            double remaining = FlightPathMathHelper.CalculateDistance(nextLocation, _mission.Destination);

            _logger.LogInformation(
                "[{Time:HH:mm:ss}] UAV {Id} Pos {Lat:F6},{Lon:F6},{Alt:F1}m Spd {Spd:F1}m/s Remain {Rem:F1}m",
                DateTime.Now, _uav.Id,
                nextLocation.Latitude, nextLocation.Longitude, nextLocation.Altitude,
                speed, remaining);

            if (remaining <= SimulationConstants.FlightPath.LOCATION_PRECISION)
                CompleteMission(nextLocation);
            else
                LocationUpdated?.Invoke(nextLocation);
        }

        private void UpdateTelemetryFields(double latitude, double longitude, double altitude)
        {
            _uav.TelemetryData[TelemetryFields.LocationLatitude] = latitude;
            _uav.TelemetryData[TelemetryFields.LocationLongitude] = longitude;
            _uav.TelemetryData[TelemetryFields.LocationAltitudeAmsl] = altitude;

        }

        private void CompleteMission(Location finalLocation)
        {
            _logger.LogInformation(
                "MISSION COMPLETED at {Lat:F8},{Lon:F8}",
                finalLocation.Latitude, finalLocation.Longitude);

            StopTracking();
            MissionCompleted?.Invoke();
        }

        public void Dispose()
        {
            _updateTimer.Dispose();
        }
    }
}
