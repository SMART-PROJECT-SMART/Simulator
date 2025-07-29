using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;
using Simulation.Models;

namespace Simulation.Services.Flight_Path.Orientation_Calculator
{
    public class OrientationCalculator : IOrientationCalculator
    {
        private double _lastYaw = double.NaN;

        public AxisDegrees ComputeOrientation(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            double deltaSec)
        {
            double speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);

            double bearing = FlightPathMathHelper.CalculateBearing(previous, current);
            double distance = FlightPathMathHelper.CalculateDistance(previous, current);
            double altDiff = current.Altitude - previous.Altitude;
            double pitch = distance > 0
                ? UnitConversionHelper.ToDegrees(Math.Atan2(altDiff, distance))
                : 0.0;

            double roll = 0.0;
            if (!double.IsNaN(_lastYaw))
            {
                double deltaYaw = FlightPathMathHelper.NormalizeAngle(bearing - _lastYaw);
                double yawRate = UnitConversionHelper.ToRadians(deltaYaw) / deltaSec;
                double speedMps = speedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;

                if (Math.Abs(yawRate) > SimulationConstants.FlightPath.MIN_YAW_RATE
                    && speedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
                {
                    double latAcc = speedMps * yawRate;
                    roll = UnitConversionHelper.ToDegrees(
                        Math.Atan2(latAcc, SimulationConstants.Mathematical.GRAVITY));
                    roll = Math.Clamp(roll, -SimulationConstants.FlightPath.MAX_ROLL_DEG, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                }
            }

            _lastYaw = bearing;
            telemetry[TelemetryFields.YawDeg] = bearing;
            telemetry[TelemetryFields.PitchDeg] = pitch;
            telemetry[TelemetryFields.RollDeg] = roll;

            return new AxisDegrees(bearing, pitch, roll);
        }
    }
}