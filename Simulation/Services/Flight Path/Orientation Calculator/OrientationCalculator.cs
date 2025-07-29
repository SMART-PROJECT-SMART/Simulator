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
            Location destination,
            double deltaSec)
        {
            double speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double distance = FlightPathMathHelper.CalculateDistance(previous, current);
            double altDiff = current.Altitude - previous.Altitude;
            double pitch = distance > 0
                ? UnitConversionHelper.ToDegrees(Math.Atan2(altDiff, distance))
                : 0.0;

            Console.WriteLine($"DEBUG ANGLES: Bearing={bearing:F1}°, Distance={distance:F1}m, AltDiff={altDiff:F1}m, Pitch={pitch:F1}°, LastYaw={_lastYaw:F1}°");

            double roll = 0.0;
            if (!double.IsNaN(_lastYaw))
            {
                double deltaYaw = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, bearing);
                double yawRate = UnitConversionHelper.ToRadians(deltaYaw) / deltaSec;
                double speedMps = speedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;

                Console.WriteLine($"DEBUG ROLL: DeltaYaw={deltaYaw:F3}°, YawRate={yawRate:F3}rad/s, MinYawRate={SimulationConstants.FlightPath.MIN_YAW_RATE:F3}, SpeedMps={speedMps:F1}, MinSpeed={SimulationConstants.FlightPath.MIN_SPEED_MPS:F1}");

                if (Math.Abs(yawRate) > SimulationConstants.FlightPath.MIN_YAW_RATE
                    && speedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
                {
                    double latAcc = speedMps * yawRate;
                    roll = UnitConversionHelper.ToDegrees(
                        Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2));
                    roll = Math.Clamp(roll, -SimulationConstants.FlightPath.MAX_ROLL_DEG, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                    Console.WriteLine($"DEBUG ROLL: LatAcc={latAcc:F3}m/s², Roll={roll:F1}°");
                }
                else
                {
                    roll = 0.0;
                    Console.WriteLine($"DEBUG ROLL: No roll - YawRate too small or speed too low");
                }
            }



            _lastYaw = bearing;

            return new AxisDegrees(bearing, pitch, roll);
        }
    }
}