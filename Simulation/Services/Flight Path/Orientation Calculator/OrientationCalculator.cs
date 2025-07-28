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
        double pitch = UnitConversionHelper.ToDegrees(Math.Atan2(altDiff, stepDistance));

        double roll = 0.0;
        if (!double.IsNaN(_lastYaw))
        {
            double deltaYawDeg = ((bearing - _lastYaw + 540) % 360) - 180;
            double yawRateRad = UnitConversionHelper.ToRadians(deltaYawDeg) / deltaSec;
            double speedMps = speedKmph * 1000.0 / 3600.0;
            double latAcc = speedMps * yawRateRad;
            roll = UnitConversionHelper.ToDegrees(Math.Atan2(latAcc, 9.81));
        }

        _lastYaw = bearing;
        return (bearing, pitch, roll);
    }
}