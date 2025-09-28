using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Services.Flight_Path.Orientation_Calculator.Helpers
{
    public class YawCalculator
    {
        private double _lastYaw = double.NaN;

        public double CalculateNewYaw(double currentYaw, double targetYaw, double deltaSec)
        {
            double diff = FlightPathMathHelper.CalculateAngleDifference(currentYaw, targetYaw);
            double turnRate = SimulationConstants.FlightPath.MAX_TURN_RATE_DEG_PER_SEC;
            double maxDelta = turnRate * deltaSec;

            double newYaw = Math.Abs(diff) <= maxDelta ? targetYaw : currentYaw + Math.Sign(diff) * maxDelta;
            return newYaw.NormalizeAngle();
        }

        public double ApplyYawSmoothing(double targetYaw, double deltaSec)
        {
            if (double.IsNaN(_lastYaw))
                return targetYaw;

            double maxYawRate = SimulationConstants.FlightPath.MAX_TURN_RATE_DEG_PER_SEC * SimulationConstants.FlightPath.YAW_SMOOTHING_FACTOR;
            double maxYawDelta = maxYawRate * deltaSec;
            double yawDiff = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, targetYaw);

            if (Math.Abs(yawDiff) <= SimulationConstants.FlightPath.MIN_RELEVENT_YAW)
                return _lastYaw;

            if (Math.Abs(yawDiff) <= maxYawDelta)
                return targetYaw;

            double newYaw = _lastYaw + Math.Sign(yawDiff) * maxYawDelta;
            return newYaw.NormalizeAngle();
        }

        public void UpdateLastYaw(double yaw)
        {
            _lastYaw = yaw;
        }

        public double GetLastYaw() => _lastYaw;
    }
}
