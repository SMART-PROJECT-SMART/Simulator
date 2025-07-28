using Simulation.Common.constants;
using Simulation.Models.Mission;

namespace Simulation.Services.helpers
{
    public class FlightPathMathHelper
    {
        public static double CalculateBearing(Location from, Location to)
        {
            double fromLatRad = UnitConversionHelper.ToRadians(from.Latitude);
            double toLatRad = UnitConversionHelper.ToRadians(to.Latitude);
            double deltaLonRad = UnitConversionHelper.ToRadians(to.Longitude - from.Longitude);

            double y = Math.Sin(deltaLonRad) * Math.Cos(toLatRad);
            double x = Math.Cos(fromLatRad) * Math.Sin(toLatRad)
                       - Math.Sin(fromLatRad) * Math.Cos(toLatRad) * Math.Cos(deltaLonRad);
            double bearingRadians = Math.Atan2(y, x);
            double bearingDegrees = UnitConversionHelper.ToDegrees(bearingRadians);
            return (bearingDegrees + 360) % 360;
        }
        public static double CalculateDistance(Location a, Location b)
        {
            double lat1Rad = UnitConversionHelper.ToRadians(a.Latitude);
            double lat2Rad = UnitConversionHelper.ToRadians(b.Latitude);
            double deltaLatRad = lat2Rad - lat1Rad;
            double deltaLonRad = UnitConversionHelper.ToRadians(b.Longitude - a.Longitude);

            double sinDeltaLat = Math.Sin(deltaLatRad / 2);
            double sinDeltaLon = Math.Sin(deltaLonRad / 2);
            double haversine = sinDeltaLat * sinDeltaLat
                          + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * sinDeltaLon * sinDeltaLon;
            double angularDistance = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));
            return SimulationConstants.FlightPath.EARTH_RADIUS_METERS * angularDistance;
        }
        public static Location CalculateDestinationLocation(Location origin, double bearing, double distance)
        {
            double originLatRad = UnitConversionHelper.ToRadians(origin.Latitude);
            double originLonRad = UnitConversionHelper.ToRadians(origin.Longitude);
            double bearingRad = UnitConversionHelper.ToRadians(bearing);
            double angularDistance = distance / SimulationConstants.FlightPath.EARTH_RADIUS_METERS;

            double destLatRad = Math.Asin(
                Math.Sin(originLatRad) * Math.Cos(angularDistance)
                + Math.Cos(originLatRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad)
            );

            double destLonRad = originLonRad + Math.Atan2(
                Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(originLatRad),
                Math.Cos(angularDistance) - Math.Sin(originLatRad) * Math.Sin(destLatRad)
            );

            return new Location(
                UnitConversionHelper.ToDegrees(destLatRad),
                UnitConversionHelper.ToDegrees(destLonRad),
                origin.Altitude
            );
        }

    }
}
