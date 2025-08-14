using Simulation.Common.constants;

namespace Simulation.Services.Flight_Path.Orientation_Calculator.Helpers
{
    public class AxisSmoothingHelper
    {
        public double ApplyAxisSmoothing(double targetValue, double lastValue, double maxRate, double deltaSec)
        {
            double maxDelta = maxRate * deltaSec;
            double diff = targetValue - lastValue;

            if (IsNegligibleDifference(diff))
                return lastValue;

            if (Math.Abs(diff) <= maxDelta)
                return targetValue;

            return lastValue + Math.Sign(diff) * maxDelta;
        }

        private bool IsNegligibleDifference(double diff)
        {
            return Math.Abs(diff) <= SimulationConstants.Mathematical.EPSILON * 1000;
        }
    }
}
