using JetBrains.Annotations;

namespace Simulation.Common.constants
{
    public static class SimulationConstants
    {
        public static class Hermes900_Constants
        {
            public const double MaxAcceleration = 2.0;
            public const double MaxDeceleration = 2.8;
            public const double MaxCruiseSpeedKmph = 220;
            public const double CruiseAltitude = 900;
            public const double FuelTankSize = 350;
            public const double Mass = 1100.0;
            public const double FrontalSurface = 4.8;
            public const double WingsSurface = 15.2;
            public const double DragCoefficient = 0.022;
            public const double LiftCoefficient = 0.85;
            public const double ThrustMax = 120.0;
            public const double ThrustAfterInfluence = 120.0;
            public const double HellfireAmmo = 2.0;
            public const double SpikeNLOSAmmo = 2.0;
            public const double GriffinAmmo = 1.0;
        }

        public static class HeronTP_Constants
        {
            public const double MaxAcceleration = 1.5;
            public const double MaxDeceleration = 2.0;
            public const double MaxCruiseSpeedKmph = 220;
            public const double CruiseAltitude = 135;
            public const double FuelTankSize = 450;
            public const double Mass = 4650.0;
            public const double FrontalSurface = 8.2;
            public const double WingsSurface = 26.0;
            public const double DragCoefficient = 0.018;
            public const double LiftCoefficient = 0.9;
            public const double ThrustMax = 1200.0;
            public const double ThrustAfterInfluence = 1200.0;
            public const double HellfireAmmo = 4.0;
            public const double GriffinAmmo = 2.0;
            public const double JDAMAmmo = 1.0;
        }

        public static class Hermes450_Constants
        {
            public const double MaxAcceleration = 2.5;
            public const double MaxDeceleration = 3.0;
            public const double MaxCruiseSpeedKmph = 220;
            public const double CruiseAltitude = 550;
            public const double FuelTankSize = 180;
            public const double DataStorageCapacityGB = 500;
            public const double Mass = 450.0;
            public const double FrontalSurface = 2.5;
            public const double WingsSurface = 8.5;
            public const double DragCoefficient = 0.025;
            public const double LiftCoefficient = 0.8;
            public const double ThrustMax = 52.0;
            public const double ThrustAfterInfluence = 52.0;
        }

        public static class Searcher_Constants
        {
            public const double MaxAcceleration = 3.0;
            public const double MaxDeceleration = 3.5;
            public const double MaxCruiseSpeedKmph = 180;
            public const double CruiseAltitude = 600;
            public const double FuelTankSize = 120;
            public const double DataStorageCapacityGB = 250;
            public const double Mass = 120.0;
            public const double FrontalSurface = 1.2;
            public const double WingsSurface = 4.8;
            public const double DragCoefficient = 0.03;
            public const double LiftCoefficient = 0.75;
            public const double ThrustMax = 150.0;
            public const double ThrustAfterInfluence = 150.0;
        }

        public static class FlightPath
        {
            public static double EARTH_SCALE_HEIGHT = 8450;
            public static double EARTH_RADIUS_METERS = 6371000.0;
            public static double LOCATION_PRECISION_KM = 0.01;
            public static double Location_PRECISION_M = 10;
            public static double MIN_SPEED_KMH = 5.0;
            public static double MAX_PITCH_DEG = 30.0;
            public static double MIN_DISTANCE_M = 0.1;
            public static double CLOSE_DISTANCE_M = 50.0;
            public static double PITCH_CLIMB_DEG = 15.0;
            public static double PITCH_DESCENT_DEG = 15.0;
            public static double ALTITUDE_TOLERANCE = 0.5;
            public static double ALTITUDE_PRECISION_M = 1.0;
            public static double MIN_DESCENT_DISTANCE_KM = 0.1;
            public static double DELTA_SECONDS = 1.0;
            public static double MAX_ROLL_DEG = 45.0;
            public static double GRAVITY_MPS2 = 9.81;
            public static double MIN_YAW_RATE = 0.001;
            public static double MIN_SPEED_MPS = 1.0;

            public static double MAX_CLIMB_RATE_MPS = 10.0;
            public static double MAX_DESCENT_RATE_MPS = 10.0;
            public static double MAX_CLIMB_DEG = 20.0;
            public static double MAX_DESCENT_DEG = 20.0;

            public static double MAX_TURN_RATE_DEG_PER_SEC = 8.0;
            public static double TURN_PROGRESS_NORMALIZATION_DEG = 90.0;
            public static double TURN_START_PHASE_THRESHOLD = 0.3;
            public static double TURN_END_PHASE_THRESHOLD = 0.7;

            public static double CURVE_ROLL_THRESHOLD_DEG = 1.0;
            public static double CURVE_ROLL_MULTIPLIER = 0.5;
            public static double MAX_CURVE_ROLL_DEG = 3.0;
            public static double MIN_ROLL_FOR_CURVE_DEG = 2.0;

            public static double SPEED_PROGRESS_HIGH_THRESHOLD = 0.7;
            public static double SPEED_PROGRESS_LOW_THRESHOLD = 0.3;
            public static double HIGH_SPEED_DECELERATION_FACTOR = 0.5;
            public static double LOW_SPEED_ACCELERATION_FACTOR = 0.7;
            public static double LOW_SPEED_ACCELERATION_RANGE = 0.3;
            public static double NORMAL_ACCELERATION_MULTIPLIER = 1.0;
            public static double FULL_ACCELERATION_MULTIPLIER = 1.0;
            public static double FULL_DECELERATION_MULTIPLIER = 1.0;

            public static double DRAG_EFFECT_ON_ALTITUDE = 0.1;
            public static double MIN_RELEVENT_YAW = 0.1;
            public static double FULL_TURN = 1.0;
            public static double MIN_PITCH = 0.2;
        }

        public static class Mathematical
        {
            public static double GRAVITY = 9.81;
            public static double EPSILON = 1e-10;
            public static double RHO = 1.225;
            public static double CRITICAL_MACH_NUMBER = 0.78;
            public static double MAXIMAL_KINETIC_ENERGY_FOR_LANDING = 1323000;
            public static double SPEED_OF_SOUND = 343.2;
            public static double FROM_KMH_TO_MPS = 3.6;
            public static double FROM_MPS_TO_KMH = 3.6;
            public static double FROM_M_TO_KM = 0.0001;
            public static int FULL_TURN_DEGREES = 360;
            public static int HALF_TURN_DEGREES = 180;
            public static double MAX_HAVESINE_RANGE = 1.0;
            public static double MIN_ACCELERATION_FACTOR = 0.1;
            public static double REALISTIC_STOP_PRECENT = 0.3;
        }
    }
}
