using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Factories.Flight_Phase;

public record PhaseDetails(Location TargetLocation, FlightPhase Phase, double PitchDegrees);

public static class FlightPhaseFactory
{
    public static PhaseDetails DeterminePhaseDetails(
        Location current,
        Location destination,
        double cruiseAltitude)
    {
        double remainingKm = FlightPathMathHelper.CalculateDistance(current, destination) / 1000.0;
        double altitudeDiff = destination.Altitude - current.Altitude;
        double currentToCruiseDiff = cruiseAltitude - current.Altitude;

        if (remainingKm <= SimulationConstants.FlightPath.LOCATION_PRECISION_KM * 10)
        {
            if (System.Math.Abs(altitudeDiff) <= SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
                return new PhaseDetails(
                    new Location(destination.Latitude, destination.Longitude, destination.Altitude),
                    FlightPhase.Cruise,
                    0.0);

            double finalPitch = CalculatePitchAngle(
                altitudeDiff,
                System.Math.Max(remainingKm * 1000.0, 100.0));

            var finalPhase = altitudeDiff > 0
                ? FlightPhase.Climb
                : FlightPhase.Descent;

            return new PhaseDetails(
                new Location(destination.Latitude, destination.Longitude, destination.Altitude),
                finalPhase,
                finalPitch);
        }

        if (System.Math.Abs(destination.Altitude - cruiseAltitude) > SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            double transitionDistance = System.Math.Abs(destination.Altitude - cruiseAltitude)
                / System.Math.Tan(UnitConversionHelper.ToRadians(15.0))
                / 1000.0;

            if (remainingKm <= transitionDistance + SimulationConstants.FlightPath.MIN_DESCENT_DISTANCE_KM)
            {
                double transitionPitch = CalculatePitchAngle(altitudeDiff, remainingKm * 1000.0);

                var transitionPhase = altitudeDiff > 0
                    ? FlightPhase.Climb
                    : FlightPhase.Descent;

                return new PhaseDetails(
                    new Location(destination.Latitude, destination.Longitude, destination.Altitude),
                    transitionPhase,
                    transitionPitch);
            }
        }

        if (ShouldStartDescent(current, destination, remainingKm))
        {
            double descentPitch = CalculatePitchAngle(altitudeDiff, remainingKm * 1000.0);

            return new PhaseDetails(
                new Location(destination.Latitude, destination.Longitude, destination.Altitude),
                FlightPhase.Descent,
                descentPitch);
        }

        if (currentToCruiseDiff > SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
        {
            double climbPitch = System.Math.Min(
                SimulationConstants.FlightPath.PITCH_CLIMB_DEG,
                CalculatePitchAngle(currentToCruiseDiff, remainingKm * 1000.0));

            return new PhaseDetails(
                new Location(destination.Latitude, destination.Longitude, cruiseAltitude),
                FlightPhase.Climb,
                climbPitch);
        }

        return new PhaseDetails(
            new Location(destination.Latitude, destination.Longitude, cruiseAltitude),
            FlightPhase.Cruise,
            0.0);
    }

    private static bool ShouldStartDescent(Location current, Location destination, double remainingKm)
    {
        double altitudeToLose = current.Altitude - destination.Altitude;
        if (altitudeToLose <= SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
            return false;

        double descentAngleRad = UnitConversionHelper.ToRadians(SimulationConstants.FlightPath.PITCH_DESCENT_DEG);
        double descentDistanceNeeded = (altitudeToLose / System.Math.Tan(descentAngleRad)) / 1000.0;
        return remainingKm <= (descentDistanceNeeded + SimulationConstants.FlightPath.MIN_DESCENT_DISTANCE_KM);
    }

    private static double CalculatePitchAngle(double altitudeDiff, double horizontalDistance)
    {
        if (horizontalDistance <= 0 || System.Math.Abs(altitudeDiff) < SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
            return 0.0;

        double pitchRad = System.Math.Atan2(altitudeDiff, horizontalDistance);
        double pitchDeg = UnitConversionHelper.ToDegrees(pitchRad);

        if (pitchDeg > 0)
            return System.Math.Min(pitchDeg, SimulationConstants.FlightPath.PITCH_CLIMB_DEG);
        else
            return System.Math.Max(pitchDeg, -SimulationConstants.FlightPath.PITCH_DESCENT_DEG);
    }
}
