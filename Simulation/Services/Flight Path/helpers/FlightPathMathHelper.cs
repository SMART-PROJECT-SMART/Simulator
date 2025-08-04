using Simulation.Common.constants;
using Simulation.Models;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.helpers
{
    public static class FlightPathMathHelper
    {
        public static double CalculateBearing(Location from, Location to)
        {
            double fromLatRad = from.Latitude.ToRadians();
            double toLatRad = to.Latitude.ToRadians();
            double deltaLonRad = to.Longitude.ToRadians() - from.Longitude.ToRadians();

            double y = CalculateBearingYComponent(deltaLonRad, toLatRad);
            double x = CalculateBearingXComponent(fromLatRad, toLatRad, deltaLonRad);

            if (
                Math.Abs(x) < SimulationConstants.Mathematical.EPSILON
                && Math.Abs(y) < SimulationConstants.Mathematical.EPSILON
            )
                return 0.0;

            double bearingRadians = Math.Atan2(y, x);
            double bearingDegrees = bearingRadians.ToDegrees();
            return (bearingDegrees + SimulationConstants.Mathematical.FULL_TURN_DEGREES)
                % SimulationConstants.Mathematical.FULL_TURN_DEGREES;
        }

        public static double CalculateDistance(Location a, Location b)
        {
            if (
                Math.Abs(a.Latitude - b.Latitude) < SimulationConstants.Mathematical.EPSILON
                && Math.Abs(a.Longitude - b.Longitude) < SimulationConstants.Mathematical.EPSILON
            )
            {
                return Math.Abs(b.Altitude - a.Altitude);
            }

            double lat1Rad = a.Latitude.ToRadians();
            double lat2Rad = b.Latitude.ToRadians();
            double deltaLatRad = lat2Rad - lat1Rad;
            double deltaLonRad = (b.Longitude - a.Longitude).ToRadians();

            double haversine = CalculateHaversineValue(lat1Rad, lat2Rad, deltaLatRad, deltaLonRad);
            haversine = Math.Min(1.0, Math.Max(0.0, haversine));
            double angularDistance = CalculateAngularDistance(haversine);

            double horizontalDistance =
                SimulationConstants.FlightPath.EARTH_RADIUS_METERS * angularDistance;
            double verticalDistance = b.Altitude - a.Altitude;
            return Math.Sqrt(
                horizontalDistance * horizontalDistance + verticalDistance * verticalDistance
            );
        }

        private static double CalculateAngularDistance(double haversine)
        {
            return 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));
        }

        public static Location CalculateDestinationLocation(
            Location origin,
            double bearing,
            double distance
        )
        {
            if (distance < SimulationConstants.FlightPath.MIN_DISTANCE_M)
                return origin;

            double originLatRad = origin.Latitude.ToRadians();
            double originLonRad = origin.Longitude.ToRadians();
            double bearingRad = bearing.ToRadians();
            double angularDistance = distance / SimulationConstants.FlightPath.EARTH_RADIUS_METERS;

            double destLatRad = CalculateDestinationLatitude(
                originLatRad,
                angularDistance,
                bearingRad
            );

            double destLonRad = CalculateDestinationLongitude(
                originLonRad,
                originLatRad,
                destLatRad,
                bearingRad,
                angularDistance
            );

            double destLat = destLatRad.ToDegrees();
            double destLon = destLonRad.ToDegrees();

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

        public static double GetHorizontalPart(double travelM, double pitchDeg)
        {
            return travelM * Math.Cos(pitchDeg.ToRadians());
        }

        private static double CalculateBearingYComponent(double deltaLonRad, double toLatRad)
        {
            return Math.Sin(deltaLonRad) * Math.Cos(toLatRad);
        }

        private static double CalculateBearingXComponent(
            double fromLatRad,
            double toLatRad,
            double deltaLonRad
        )
        {
            return Math.Cos(fromLatRad) * Math.Sin(toLatRad)
                - Math.Sin(fromLatRad) * Math.Cos(toLatRad) * Math.Cos(deltaLonRad);
        }

        private static double CalculateHaversineValue(
            double lat1Rad,
            double lat2Rad,
            double deltaLatRad,
            double deltaLonRad
        )
        {
            double sinDeltaLat = Math.Sin(deltaLatRad / 2);
            double sinDeltaLon = Math.Sin(deltaLonRad / 2);
            return sinDeltaLat * sinDeltaLat
                + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * sinDeltaLon * sinDeltaLon;
        }

        private static double CalculateDestinationLatitude(
            double originLatRad,
            double angularDistance,
            double bearingRad
        )
        {
            return Math.Asin(
                Math.Sin(originLatRad) * Math.Cos(angularDistance)
                    + Math.Cos(originLatRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad)
            );
        }

        private static double CalculateDestinationLongitude(
            double originLonRad,
            double originLatRad,
            double destLatRad,
            double bearingRad,
            double angularDistance
        )
        {
            return originLonRad
                + Math.Atan2(
                    Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(originLatRad),
                    Math.Cos(angularDistance) - Math.Sin(originLatRad) * Math.Sin(destLatRad)
                );
        }
    }
}
