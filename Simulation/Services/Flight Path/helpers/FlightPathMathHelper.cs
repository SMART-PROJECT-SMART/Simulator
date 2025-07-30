using Simulation.Common.constants;
using Simulation.Models;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.helpers
{
    public static class FlightPathMathHelper
    {
        public static double CalculateBearing(Location from, Location to)
        {
            double fromLatRad = UnitConversionHelper.ToRadians(from.Latitude);
            double toLatRad = UnitConversionHelper.ToRadians(to.Latitude);
            double deltaLonRad = UnitConversionHelper.ToRadians(to.Longitude - from.Longitude);

            double y = Math.Sin(deltaLonRad) * Math.Cos(toLatRad);
            double x =
                Math.Cos(fromLatRad) * Math.Sin(toLatRad)
                - Math.Sin(fromLatRad) * Math.Cos(toLatRad) * Math.Cos(deltaLonRad);

            if (
                Math.Abs(x) < SimulationConstants.Mathematical.EPSILON
                && Math.Abs(y) < SimulationConstants.Mathematical.EPSILON
            )
                return 0.0;

            double bearingRadians = Math.Atan2(y, x);
            double bearingDegrees = UnitConversionHelper.ToDegrees(bearingRadians);
            return (bearingDegrees + SimulationConstants.Mathematical.FULL_TURN_DEGREES)
                % SimulationConstants.Mathematical.FULL_TURN_DEGREES;
        }

        public static double CalculateDistance(Location a, Location b)
        {
            if (
                Math.Abs(a.Latitude - b.Latitude) < SimulationConstants.Mathematical.EPSILON
                && Math.Abs(a.Longitude - b.Longitude) < SimulationConstants.Mathematical.EPSILON
            )
                return 0.0;

            double lat1Rad = UnitConversionHelper.ToRadians(a.Latitude);
            double lat2Rad = UnitConversionHelper.ToRadians(b.Latitude);
            double deltaLatRad = lat2Rad - lat1Rad;
            double deltaLonRad = UnitConversionHelper.ToRadians(b.Longitude - a.Longitude);

            double sinDeltaLat = Math.Sin(deltaLatRad / 2);
            double sinDeltaLon = Math.Sin(deltaLonRad / 2);
            double haversine =
                sinDeltaLat * sinDeltaLat
                + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * sinDeltaLon * sinDeltaLon;

            if (haversine >= SimulationConstants.Mathematical.MAX_HAVESINE_RANGE)
                return Math.PI * SimulationConstants.FlightPath.EARTH_RADIUS_METERS;

            double angularDistance =
                2
                * Math.Atan2(
                    Math.Sqrt(haversine),
                    Math.Sqrt(SimulationConstants.Mathematical.MAX_HAVESINE_RANGE - haversine)
                );
            return SimulationConstants.FlightPath.EARTH_RADIUS_METERS * angularDistance;
        }

        public static Location CalculateDestinationLocation(
            Location origin,
            double bearing,
            double distance
        )
        {
            if (distance < SimulationConstants.FlightPath.MIN_DISTANCE_M)
                return origin;

            double originLatRad = UnitConversionHelper.ToRadians(origin.Latitude);
            double originLonRad = UnitConversionHelper.ToRadians(origin.Longitude);
            double bearingRad = UnitConversionHelper.ToRadians(bearing);
            double angularDistance = distance / SimulationConstants.FlightPath.EARTH_RADIUS_METERS;

            double destLatRad = Math.Asin(
                Math.Sin(originLatRad) * Math.Cos(angularDistance)
                    + Math.Cos(originLatRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad)
            );

            double destLonRad =
                originLonRad
                + Math.Atan2(
                    Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(originLatRad),
                    Math.Cos(angularDistance) - Math.Sin(originLatRad) * Math.Sin(destLatRad)
                );

            double destLat = UnitConversionHelper.ToDegrees(destLatRad);
            double destLon = UnitConversionHelper.ToDegrees(destLonRad);

            destLon =
                (
                    (destLon % SimulationConstants.Mathematical.FULL_TURN_DEGREES)
                    + SimulationConstants.Mathematical.FULL_TURN_DEGREES
                ) % SimulationConstants.Mathematical.FULL_TURN_DEGREES;

            return new Location(destLat, destLon, origin.Altitude);
        }

        public static double CalculateAngleDifference(double angle1, double angle2)
        {
            double diff = angle2 - angle1;

            while (diff > SimulationConstants.Mathematical.HALF_TURN_DEGREES)
            {
                diff -= SimulationConstants.Mathematical.FULL_TURN_DEGREES;
            }
            while (diff < -SimulationConstants.Mathematical.HALF_TURN_DEGREES)
            {
                diff += SimulationConstants.Mathematical.FULL_TURN_DEGREES;
            }

            return diff;
        }

        public static double NormalizeAngle(this double angle)
        {
            angle =
                (angle + SimulationConstants.Mathematical.HALF_TURN_DEGREES)
                % SimulationConstants.Mathematical.FULL_TURN_DEGREES;
            if (angle < 0)
            {
                angle += SimulationConstants.Mathematical.FULL_TURN_DEGREES;
            }
            return angle - SimulationConstants.Mathematical.HALF_TURN_DEGREES;
        }

        public static double ToKmhFromMps(this double speedKmh)
        {
            return speedKmh / SimulationConstants.Mathematical.FROM_MPS_TO_KMH;
        }

        public static double ToMpsFromKmh(this double speedMps)
        {
            return speedMps * SimulationConstants.Mathematical.FROM_MPS_TO_KMH;
        }

        public static double FromMToKm(this double meters)
        {
            return meters * SimulationConstants.Mathematical.FROM_M_TO_KM;
        }
    }
}
