namespace Simulation.Common.constants
{
    public static class SimulationConstants
    {
        public static class Hermes900_Constants
        {
            public const double MaxAcceleration = 2.0;
            public const double MaxDeceleration = 2.8;
            public const double MaxCruiseSpeedKmph = 220;
            public const double CruiseAltitude = 900.0;
            public const double FuelTankCapacity = 350.0;
            public const double FuelConsumption = 1.8e-4;
            public const double Mass = 1100.0;
            public const double FrontalSurface = 4.8;
            public const double WingsSurface = 15.2;
            public const double DragCoefficient = 0.002;
            public const double LiftCoefficient = 0.85;
            public const double ThrustMax = 120.0;
            public const double ThrustAfterInfluence = 120.0;
            public const double HellfireAmmo = 2.0;
            public const double SpikeNLOSAmmo = 2.0;
            public const double GriffinAmmo = 1.0;
            public const double TransmitPowerDbm = 60.0;
            public const double TransmitAntennaGainDbi = 25.0;
            public const double ReceiveAntennaGainDbi = 30.0;
            public const double TransmitLossDb = 1.0;
            public const double ReceiveLossDb = 1.0;
            public const double FrequencyHz = 400e6;
            public const double PropellerRadius = 1.015;
            public const double BladeCircumference = 6.38;
        }

        public static class HeronTP_Constants
        {
            public const double MaxAcceleration = 1.5;
            public const double MaxDeceleration = 2.0;
            public const double MaxCruiseSpeedKmph = 220;
            public const double CruiseAltitude = 135.0;
            public const double FuelTankCapacity = 450.0;
            public const double FuelConsumption = 2.0e-4;
            public const double Mass = 4650.0;
            public const double FrontalSurface = 8.2;
            public const double WingsSurface = 26.0;
            public const double DragCoefficient = 0.0018;
            public const double LiftCoefficient = 0.9;
            public const double ThrustMax = 1200.0;
            public const double ThrustAfterInfluence = 1200.0;
            public const double HellfireAmmo = 4.0;
            public const double GriffinAmmo = 2.0;
            public const double JDAMAmmo = 1.0;
            public const double TransmitPowerDbm = 60.0;
            public const double TransmitAntennaGainDbi = 25.0;
            public const double ReceiveAntennaGainDbi = 30.0;
            public const double TransmitLossDb = 1.0;
            public const double ReceiveLossDb = 1.0;
            public const double FrequencyHz = 400e6;
            public const double PropellerRadius = 1.40;
            public const double BladeCircumference = 8.80;
        }

        public static class Hermes450_Constants
        {
            public const double MaxAcceleration = 2.5;
            public const double MaxDeceleration = 3.0;
            public const double MaxCruiseSpeedKmph = 220;
            public const double CruiseAltitude = 550.0;
            public const double FuelTankCapacity = 180.0;
            public const double FuelConsumption = 1.5e-4;
            public const double DataStorageCapacityGB = 500.0;
            public const double Mass = 450.0;
            public const double FrontalSurface = 2.5;
            public const double WingsSurface = 8.5;
            public const double DragCoefficient = 0.0025;
            public const double LiftCoefficient = 0.8;
            public const double ThrustMax = 52.0;
            public const double ThrustAfterInfluence = 52.0;
            public const double TransmitPowerDbm = 60.0;
            public const double TransmitAntennaGainDbi = 25.0;
            public const double ReceiveAntennaGainDbi = 30.0;
            public const double TransmitLossDb = 1.0;
            public const double ReceiveLossDb = 1.0;
            public const double FrequencyHz = 400e6;
            public const double PropellerRadius = 0.90;
            public const double BladeCircumference = 5.65;
        }

        public static class Searcher_Constants
        {
            public const double MaxAcceleration = 3.0;
            public const double MaxDeceleration = 3.5;
            public const double MaxCruiseSpeedKmph = 180;
            public const double CruiseAltitude = 600.0;
            public const double FuelTankCapacity = 120.0;
            public const double FuelConsumption = 1.2e-4;
            public const double DataStorageCapacityGB = 250.0;
            public const double Mass = 120.0;
            public const double FrontalSurface = 1.2;
            public const double WingsSurface = 4.8;
            public const double DragCoefficient = 0.001;
            public const double LiftCoefficient = 0.75;
            public const double ThrustMax = 150.0;
            public const double ThrustAfterInfluence = 150.0;
            public const double TransmitPowerDbm = 60.0;
            public const double TransmitAntennaGainDbi = 25.0;
            public const double ReceiveAntennaGainDbi = 30.0;
            public const double TransmitLossDb = 1.0;
            public const double ReceiveLossDb = 1.0;
            public const double FrequencyHz = 400e6;
            public const double PropellerRadius = 0.60;
            public const double BladeCircumference = 3.77;
        }

        public static class FlightPath
        {
            public const double EARTH_SCALE_HEIGHT = 8450.0;
            public const double EARTH_RADIUS_METERS = 6371000.0;
            public const double LOCATION_PRECISION_KM = 0.01;
            public const double LOCATION_PRECISION_M = 10.0;
            public const double MIN_SPEED_KMH = 5.0;
            public const double MAX_PITCH_DEG = 30.0;
            public const double MIN_DISTANCE_M = 0.1;
            public const double CLOSE_DISTANCE_M = 50.0;
            public const double PITCH_CLIMB_DEG = 15.0;
            public const double PITCH_DESCENT_DEG = 15.0;
            public const double ALTITUDE_TOLERANCE = 0.2;
            public const double ALTITUDE_PRECISION_M = 0.2;
            public const double MIN_DESCENT_DISTANCE_KM = 0.1;
            public const double DELTA_SECONDS = 1.0;
            public const double MAX_ROLL_DEG = 45.0;
            public const double GRAVITY_MPS2 = 9.81;
            public const double MIN_YAW_RATE = 0.001;
            public const double MIN_SPEED_MPS = 1.0;
            public const double MAX_CLIMB_RATE_MPS = 10.0;
            public const double MAX_DESCENT_RATE_MPS = 10.0;
            public const double MAX_CLIMB_DEG = 20.0;
            public const double MAX_DESCENT_DEG = 20.0;
            public const double MAX_TURN_RATE_DEG_PER_SEC = 8.0;
            public const double MAX_ROLL_RATE_DEG_PER_SEC = 8.0;
            public const double TURN_PROGRESS_NORMALIZATION_DEG = 90.0;
            public const double TURN_START_PHASE_THRESHOLD = 0.3;
            public const double TURN_END_PHASE_THRESHOLD = 0.7;
            public const double CURVE_ROLL_THRESHOLD_DEG = 1.0;
            public const double CURVE_ROLL_MULTIPLIER = 0.5;
            public const double MAX_CURVE_ROLL_DEG = 3.0;
            public const double MIN_ROLL_FOR_CURVE_DEG = 2.0;
            public const double SPEED_PROGRESS_HIGH_THRESHOLD = 0.7;
            public const double SPEED_PROGRESS_LOW_THRESHOLD = 0.3;
            public const double HIGH_SPEED_DECELERATION_FACTOR = 0.5;
            public const double LOW_SPEED_ACCELERATION_FACTOR = 0.7;
            public const double LOW_SPEED_ACCELERATION_RANGE = 0.3;
            public const double NORMAL_ACCELERATION_MULTIPLIER = 1.0;
            public const double FULL_ACCELERATION_MULTIPLIER = 1.0;
            public const double FULL_DECELERATION_MULTIPLIER = 1.0;
            public const double DRAG_EFFECT_ON_ALTITUDE = 0.1;
            public const double MIN_RELEVENT_YAW = 0.1;
            public const double FULL_TURN = 1.0;
            public const double MIN_PITCH = 0.2;
            public const double MIN_PITCH_REDUCTION_FACTOR = 0.6;
            public const double PITCH_REDUCTION_RATE = 0.3;
            public const double ALTITUDE_HOLD_THRESHOLD = 2.0;
            public const double ALTITUDE_HOLD_GAIN = 8.0;
            public const double MISSION_COMPLETION_RADIUS_M = 10.0;
            public const double ALTITUDE_HOLD_TIME_TO_TARGET_SEC = 0.5;
            public const int OVERHEAT = 300;

            public const string ABORT_REASON_FUEL_DEPLETION =
                "FUEL DEPLETION - Critical fuel exhaustion";
            public const string ABORT_REASON_COMMUNICATION_LOSS =
                "COMMUNICATION LOSS - Signal strength below operational threshold";
            public const string ABORT_REASON_ENGINE_OVERHEAT =
                "ENGINE OVERHEAT - Critical temperature exceeded";
            public const string ABORT_REASON_UNKNOWN =
                "UNKNOWN - Mission terminated for unspecified reason";
        }

        public static class Mathematical
        {
            public const int GIGA = 1000000000;
            public const double EPSILON = 1e-10;
            public const double RHO = 1.225;
            public const double FROM_KMH_TO_MPS = 3.6;
            public const double FROM_MPS_TO_KMH = 3.6;
            public const double FROM_M_TO_KM = 0.0001;
            public const int FULL_TURN_DEGREES = 360;
            public const int HALF_TURN_DEGREES = 180;
            public const double MIN_ACCELERATION_FACTOR = 0.1;
            public const int SPEED_OF_LIGHT_MPS = 299792458;
        }

        public static class TelemetryData
        {
            public const double WHEELS_UP = 0;
            public const double WHEELS_DOWN = 1;
            public const double NO_SIGNAL = -120.5;
            public const int BYTES_PER_FIELD = 8;
        }

        public static class Quartz
        {
            public const string UAV_ID = "UAVId";
            public const string IDENTITY_PREFIX = "FlightPath-";
            public const string JOB_GROUP = "FlightPathJob";
        }
    }
}
