using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public class SpeedCalculator : ISpeedController
    {
        public double ComputeNextSpeed(
            Dictionary<TelemetryFields, double> telemetry,
            double remainingKm,
            double deltaSeconds)
        {
            double currentSpeed = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            double maxAcceleration = telemetry.GetValueOrDefault(TelemetryFields.MaxAccelerationMps2, 2.0);
            double cruiseSpeed = telemetry.GetValueOrDefault(TelemetryFields.MaxCruiseSpeedKmph, 180.0);

            double acceleration = maxAcceleration;
            
            Console.WriteLine($"DEBUG: CurrentSpeed={currentSpeed:F1}km/h, MaxAccel={maxAcceleration:F1}m/s², UsedAccel={acceleration:F3}m/s²");
            
            double deltaSpeedKmh = acceleration * deltaSeconds * SimulationConstants.Mathematical.FROM_MPS_TO_KMH;
            double newSpeed = currentSpeed + deltaSpeedKmh;

            newSpeed = Math.Clamp(newSpeed, SimulationConstants.FlightPath.MIN_SPEED_KMH, cruiseSpeed);

            telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

            return newSpeed;
        }
    }
}