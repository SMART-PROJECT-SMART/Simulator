using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.Flight_Path.Orientation_Calculator.Helpers;

namespace Simulation.Services.Flight_Path.Orientation_Calculator
{
    public class OrientationCalculator : IOrientationCalculator
    {
        private readonly PitchCalculator _pitchCalculator;
        private readonly YawCalculator _yawCalculator;
        private readonly RollCalculator _rollCalculator;
        private readonly AxisSmoothingHelper _smoothingHelper;


        public OrientationCalculator()
        {
            _pitchCalculator = new PitchCalculator();
            _yawCalculator = new YawCalculator();
            _rollCalculator = new RollCalculator();
            _smoothingHelper = new AxisSmoothingHelper();
        }

        public AxisDegrees ComputeOrientation(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            Location destination,
            double deltaSec
        )
        {
            double smoothedPitch = CalculateAndSmoothPitch(telemetry, current, destination, deltaSec);
            double smoothedYaw = CalculateAndSmoothYaw(telemetry, current, destination, deltaSec);
            double smoothedRoll = CalculateAndSmoothRoll(telemetry, current, destination, deltaSec, smoothedYaw);
            return new AxisDegrees(smoothedYaw, smoothedPitch, smoothedRoll);
        }

        private double CalculateAndSmoothPitch(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec
        )
        {
            double targetPitch = _pitchCalculator.CalculatePhysicsBasedPitch(telemetry, current, destination);
            return targetPitch;
        }

        private double CalculateAndSmoothYaw(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec
        )
        {
            double currentYaw = telemetry.GetValueOrDefault(TelemetryFields.YawDeg, 0.0);
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double targetYaw = _yawCalculator.CalculateNewYaw(currentYaw, bearing, deltaSec);
            return _yawCalculator.ApplyYawSmoothing(targetYaw, deltaSec);
        }

        private double CalculateAndSmoothRoll(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec,
            double newYaw
        )
        {
            double targetRoll = _rollCalculator.CalculatePhysicsBasedRoll(
                telemetry, 
                current, 
                destination, 
                deltaSec, 
                newYaw, 
                _yawCalculator.GetLastYaw()
            );
            return targetRoll;
        }

        private void UpdateLastValues(double yaw, double pitch, double roll)
        {
            _yawCalculator.UpdateLastYaw(yaw);
        }
    }
}