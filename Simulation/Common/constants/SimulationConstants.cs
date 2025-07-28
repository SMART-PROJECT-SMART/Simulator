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
            public static double LOCATION_PRECISION_KM = 0.001;
            public static double MIN_SPEED_KMH = 5.0;
            public static double MAX_PITCH_DEG = 30.0;
            public static double MIN_DISTANCE_M = 0.1;
            public static double CLOSE_DISTANCE_M = 50.0;
            public static double PITCH_CLIMB_DEG = 15.0;
            public static double PITCH_DESCENT_DEG = 15.0;
            public static double ALTITUDE_TOLERANCE = 0.5;
            public static double MIN_DESCENT_DISTANCE_KM = 0.1;
            public static double DELTA_SECONDS = 1.0;
            public static double MAX_ROLL_DEG = 45.0;
            public static double GRAVITY_MPS2 = 9.81;
            public static double MIN_YAW_RATE = 0.001;
            public static double MIN_SPEED_MPS = 1.0;
        }
    }
}
