using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Factories.Flight_Phase;

public record PhaseDetails(FlightPhase Phase, double TargetAltitude, double PitchDegrees);

public static class FlightPhaseFactory
{
    public static PhaseDetails DeterminePhaseDetails(
        Location current,
        Location destination,
        double cruiseAltitude)
    {
        double remainingKm = FlightPathMathHelper.CalculateDistance(current, destination) / 1000.0;
        double altitudeDiff = destination.Altitude - current.Altitude;

        if (remainingKm <= SimulationConstants.FlightPath.MIN_DESCENT_DISTANCE_KM)
        {
            if (altitudeDiff > SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
            {
                return new PhaseDetails(FlightPhase.Climb, destination.Altitude, SimulationConstants.FlightPath.PITCH_CLIMB_DEG);
            }
            else if (altitudeDiff < -SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
            {
                return new PhaseDetails(FlightPhase.Descent, destination.Altitude, -SimulationConstants.FlightPath.PITCH_DESCENT_DEG);
            }
            else
            {
                return new PhaseDetails(FlightPhase.Cruise, destination.Altitude, 0.0);
            }
        }

        double altitudeToLoseM = current.Altitude - destination.Altitude;
        double descentKmNeeded = 0;
        
        if (altitudeToLoseM > SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            double descentAngleRad = UnitConversionHelper.ToRadians(SimulationConstants.FlightPath.PITCH_DESCENT_DEG);
            descentKmNeeded = (altitudeToLoseM / Math.Tan(descentAngleRad)) / 1000.0;
        }

        if (remainingKm <= descentKmNeeded + SimulationConstants.FlightPath.MIN_DESCENT_DISTANCE_KM && altitudeToLoseM > SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            return new PhaseDetails(FlightPhase.Descent, destination.Altitude, -SimulationConstants.FlightPath.PITCH_DESCENT_DEG);
        }

        if (current.Altitude < cruiseAltitude - SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            return new PhaseDetails(FlightPhase.Climb, cruiseAltitude, SimulationConstants.FlightPath.PITCH_CLIMB_DEG);
        }

        return new PhaseDetails(FlightPhase.Cruise, cruiseAltitude, 0.0);
    }

    public static double GetOptimalPitchAngle(FlightPhase phase, double currentAltitude, double targetAltitude)
    {
        double altitudeDiff = targetAltitude - currentAltitude;
        
        return phase switch
        {
            FlightPhase.Climb when altitudeDiff > SimulationConstants.FlightPath.ALTITUDE_TOLERANCE => Math.Min(SimulationConstants.FlightPath.PITCH_CLIMB_DEG, SimulationConstants.FlightPath.MAX_PITCH_DEG),
            FlightPhase.Descent when altitudeDiff < -SimulationConstants.FlightPath.ALTITUDE_TOLERANCE => Math.Max(-SimulationConstants.FlightPath.PITCH_DESCENT_DEG, -SimulationConstants.FlightPath.MAX_PITCH_DEG),
            _ => 0.0
        };
    }
}