using Simulation.Common.Enums;
using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Factories.Flight_Phase;

public record PhaseDetails(FlightPhase Phase, double TargetAltitude, double PitchDegrees);

public static class FlightPhaseFactory
{
    private const double PitchForClimbDeg = 15.0;
    private const double PitchForDescentDeg = 15.0;
    private const double AltitudeTolerance = 0.5;

    public static PhaseDetails DeterminePhaseDetails(
        Location current,
        Location destination,
        double cruiseAltitude)
    {
        double remainingKm = FlightPathMathHelper.CalculateDistance(current, destination) / 1000.0;

        double altitudeToLoseNowM = current.Altitude - destination.Altitude;
        double descentKmNeededNow = 0;
        if (altitudeToLoseNowM > 0)
        {
            descentKmNeededNow = (altitudeToLoseNowM / Math.Tan(UnitConversionHelper.ToRadians(PitchForDescentDeg))) / 1000.0;
        }

        if (remainingKm <= descentKmNeededNow)
        {
            return new PhaseDetails(FlightPhase.Descent, destination.Altitude, -PitchForDescentDeg);
        }

        if (current.Altitude < cruiseAltitude - AltitudeTolerance)
        {
            return new PhaseDetails(FlightPhase.Climb, cruiseAltitude, PitchForClimbDeg);
        }

        return new PhaseDetails(FlightPhase.Cruise, cruiseAltitude, 0.0);
    }
}