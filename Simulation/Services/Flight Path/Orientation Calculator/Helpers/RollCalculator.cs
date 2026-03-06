using Core.Models;
using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Orientation_Calculator.Helpers
{
    public class RollCalculator
    {
        public double CalculatePhysicsBasedRoll(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec,
            double newYaw,
            double lastYaw
        )
        {
            double speedMps = GetSpeedInMps(telemetry);
            if (speedMps <= SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double remainingDistance = FlightPathMathHelper.CalculateDistance(current, destination);
            if (remainingDistance <= SimulationConstants.FlightPath.CLOSE_DISTANCE_M)
                return 0.0;

            if (double.IsNaN(lastYaw))
                return 0.0;

            double targetRoll = CalculateYawBasedRoll(speedMps, newYaw, deltaSec, lastYaw);
            return CalculateCurveRoll(current, destination, newYaw, targetRoll);
        }

        private double GetSpeedInMps(Dictionary<TelemetryFields, double> telemetry)
        {
            return telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0).ToKmhFromMps();
        }

        private double CalculateYawBasedRoll(double speedMps, double newYaw, double deltaSec, double lastYaw)
        {
            double dYaw = FlightPathMathHelper.CalculateAngleDifference(lastYaw, newYaw);
            double yawRate = dYaw.ToRadians() / deltaSec;

            if (Math.Abs(yawRate) <= SimulationConstants.FlightPath.MIN_YAW_RATE)
                return 0.0;

            double latAcc = speedMps * yawRate;
            double targetRoll = Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2).ToDegrees();
            
            return Math.Clamp(
                targetRoll,
                -SimulationConstants.FlightPath.MAX_ROLL_DEG,
                SimulationConstants.FlightPath.MAX_ROLL_DEG
            );
        }

        private double CalculateCurveRoll(
            Location current,
            Location destination,
            double newYaw,
            double currentRoll
        )
        {
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double diff = FlightPathMathHelper.CalculateAngleDifference(newYaw, bearing);

            if (Math.Abs(diff) <= SimulationConstants.FlightPath.CURVE_ROLL_THRESHOLD_DEG)
                return currentRoll;

            double curveRoll = Math.Min(
                Math.Abs(diff) * SimulationConstants.FlightPath.CURVE_ROLL_MULTIPLIER,
                SimulationConstants.FlightPath.MAX_CURVE_ROLL_DEG
            );
            return currentRoll + Math.Sign(diff) * curveRoll;
        }
    }
}
