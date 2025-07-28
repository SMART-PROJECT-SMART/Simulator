namespace Simulation.Common.constants
{
    public static class SimulationConstants
    {
        public static class Configuration
        {
            public static string DB_CONNECTION = "DbConnection";
        }
        public static class Mongo
        {
            public static class Schemas
            {
                public static string UAV_SCHEMA = "uav";
                public static string MISSION_SCHEMA = "mission";
            }
        }

        public static class FlightPath
        {
            public static double EARTH_RADIUS_METERS = 6371000.0;
            public static double METERS_TO_DEGREES_LAT = 1.0 / 111320.0;
            public static double LOCATION_PRECISION = 1;
        }

    }
}
