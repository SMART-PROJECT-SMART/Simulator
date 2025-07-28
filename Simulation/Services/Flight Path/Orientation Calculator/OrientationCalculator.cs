using Simulation.Common.constants;
using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Orientation_Calculator;

public class OrientationCalculator : IOrientationCalculator
{
    private double _lastYaw = double.NaN;

    public (double yaw, double pitch, double roll) ComputeOrientation(
        Location previous,
        Location current,
        double speedKmph,
        double deltaSec)
    {
        double bearing = FlightPathMathHelper.CalculateBearing(previous, current);

        double stepDistance = FlightPathMathHelper.CalculateDistance(previous, current);
        double altDiff = current.Altitude - previous.Altitude;
        double pitch = stepDistance > 0 
            ? UnitConversionHelper.ToDegrees(Math.Atan2(altDiff, stepDistance))
            : 0.0;

        double roll = 0.0;
        if (!double.IsNaN(_lastYaw))
        {
            double deltaYawDeg = NormalizeAngle(bearing - _lastYaw);
            double yawRateRad = UnitConversionHelper.ToRadians(deltaYawDeg) / deltaSec;
            double speedMps = speedKmph * 1000.0 / 3600.0;
            
            if (Math.Abs(yawRateRad) > SimulationConstants.FlightPath.MIN_YAW_RATE && speedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
            {
                double latAcc = speedMps * yawRateRad;
                roll = UnitConversionHelper.ToDegrees(Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2));
                roll = Math.Max(-SimulationConstants.FlightPath.MAX_ROLL_DEG, Math.Min(SimulationConstants.FlightPath.MAX_ROLL_DEG, roll));
            }
        }

        _lastYaw = bearing;
        return (bearing, pitch, roll);
    }

    private static double NormalizeAngle(double angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    public void Reset()
    {
        _lastYaw = double.NaN;
    }
}