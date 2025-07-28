// FlightPhaseFactory.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Simulation.Common.Enums;
using Simulation.Models.Mission;

namespace Simulation.Factories.Flight_Phase
{
    public static class FlightPhaseFactory
    {
        private static readonly List<IFlightPhaseStrategy> _strategies = new()
        {
            new ClimbPhaseStrategy(),
            new CruisePhaseStrategy(),
            new DescentPhaseStrategy()
        };

        public static FlightPhase DeterminePhase(
            Location current,
            Location destination,
            double cruiseAltitude)
        {
            if (current.Altitude < cruiseAltitude - 0.1)
                return FlightPhase.Climb;
            if (current.Altitude > destination.Altitude + 0.1)
                return FlightPhase.Descent;
            return FlightPhase.Cruise;
        }

        public static IFlightPhaseStrategy GetStrategy(FlightPhase phase) =>
            _strategies.First(s => s.Phase == phase);
    }
}