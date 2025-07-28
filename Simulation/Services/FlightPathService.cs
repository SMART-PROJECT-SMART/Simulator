using Microsoft.Extensions.Logging;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.Mission;
using Simulation.Models.Uav;
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
        private readonly Timer _updateTimer;
        private Location _currentLocation;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        public event Action<Location>? LocationUpdated;
        public event Action? MissionCompleted;

        public FlightPathService(UAV uav, Mission mission, ILogger<FlightPathService> logger)
        {
            _uav = uav;
            _mission = mission;
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
                CalculateDistance(_currentLocation, _mission.Destination));

            _updateTimer.Change(TimeSpan.Zero, _updateInterval);
        }

        public void StopTracking()
        {
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Tracking stopped for UAV {UavId}", _uav.Id);
        }

        private void UpdateLocation(object? state)
        {
            var speed = _uav.TelemetryData.GetValueOrDefault(TelemetryFields.LocationGroundSpeed, 0.0);
            var moveDistance = speed * _updateInterval.TotalSeconds;
            var remainingDistance = CalculateDistance(_currentLocation, _mission.Destination);

            Location newLocation;

            if (moveDistance >= remainingDistance)
            {
                newLocation = new Location(
                    _mission.Destination.Latitude,
                    _mission.Destination.Longitude,
                    _mission.Destination.Altitude
                );
            }
            else
            {
                var bearingToTarget = CalculateBearing(_currentLocation, _mission.Destination);
                var nextHorizontalPosition = CalculateDestinationLocation(_currentLocation, bearingToTarget, moveDistance);

                var pitchAngle = _uav.TelemetryData.GetValueOrDefault(TelemetryFields.LocationPitch, 0.0);
                var pitchRadians = UnitConversionHelper.ToRadians(pitchAngle);
                var altitudeChange = Math.Sin(pitchRadians) * moveDistance;

                newLocation = new Location(
                    nextHorizontalPosition.Latitude,
                    nextHorizontalPosition.Longitude,
                    nextHorizontalPosition.Altitude + altitudeChange
                );
            }

            _currentLocation = newLocation;
            WriteTelemetry(newLocation);

            remainingDistance = CalculateDistance(newLocation, _mission.Destination);
            _logger.LogInformation(
                "[{Time:HH:mm:ss}] UAV {Id} Pos {Lat:F6},{Lon:F6},{Alt:F1}m Spd {Spd:F1}m/s Remain {Rem:F1}m",
                DateTime.Now, _uav.Id,
                newLocation.Latitude, newLocation.Longitude, newLocation.Altitude,
                speed, remainingDistance);

            if (remainingDistance <= SimulationConstants.FlightPath.LOCATION_PRECISION)
            {
                _logger.LogInformation(
                    "MISSION COMPLETED at {Lat:F8},{Lon:F8}", newLocation.Latitude, newLocation.Longitude);
                StopTracking();
                MissionCompleted?.Invoke();
            }
            else
            {
                LocationUpdated?.Invoke(newLocation);
            }
        }

        private void WriteTelemetry(Location position)
        {
            var telemetry = _uav.TelemetryData;
            telemetry[TelemetryFields.LocationLatitude] = position.Latitude;
            telemetry[TelemetryFields.LocationLongitude] = position.Longitude;
            telemetry[TelemetryFields.LocationAltitudeAmsl] = position.Altitude;
        }

        private double CalculateBearing(Location from, Location to)
        {
            var fromLatRad = UnitConversionHelper.ToRadians(from.Latitude);
            var toLatRad = UnitConversionHelper.ToRadians(to.Latitude);
            var deltaLonRad = UnitConversionHelper.ToRadians(to.Longitude - from.Longitude);

            var y = Math.Sin(deltaLonRad) * Math.Cos(toLatRad);
            var x = Math.Cos(fromLatRad) * Math.Sin(toLatRad)
                  - Math.Sin(fromLatRad) * Math.Cos(toLatRad) * Math.Cos(deltaLonRad);
            var bearingRadians = Math.Atan2(y, x);
            var bearingDegrees = UnitConversionHelper.ToDegrees(bearingRadians);
            return (bearingDegrees + 360) % 360;
        }

        private double CalculateDistance(Location a, Location b)
        {
            var lat1Rad = UnitConversionHelper.ToRadians(a.Latitude);
            var lat2Rad = UnitConversionHelper.ToRadians(b.Latitude);
            var deltaLatRad = lat2Rad - lat1Rad;
            var deltaLonRad = UnitConversionHelper.ToRadians(b.Longitude - a.Longitude);

            var sinDeltaLat = Math.Sin(deltaLatRad / 2);
            var sinDeltaLon = Math.Sin(deltaLonRad / 2);
            var haversine = sinDeltaLat * sinDeltaLat
                          + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * sinDeltaLon * sinDeltaLon;
            var angularDistance = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));
            return SimulationConstants.FlightPath.EARTH_RADIUS_METERS * angularDistance;
        }

        private Location CalculateDestinationLocation(Location origin, double bearing, double distance)
        {
            var originLatRad = UnitConversionHelper.ToRadians(origin.Latitude);
            var originLonRad = UnitConversionHelper.ToRadians(origin.Longitude);
            var bearingRad = UnitConversionHelper.ToRadians(bearing);
            var angularDistance = distance / SimulationConstants.FlightPath.EARTH_RADIUS_METERS;

            var destLatRad = Math.Asin(
                Math.Sin(originLatRad) * Math.Cos(angularDistance)
              + Math.Cos(originLatRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad)
            );

            var destLonRad = originLonRad + Math.Atan2(
                Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(originLatRad),
                Math.Cos(angularDistance) - Math.Sin(originLatRad) * Math.Sin(destLatRad)
            );

            return new Location(
                UnitConversionHelper.ToDegrees(destLatRad),
                UnitConversionHelper.ToDegrees(destLonRad),
                origin.Altitude
            );
        }

        public void Dispose()
        {
            _updateTimer.Dispose();
        }
    }
}
