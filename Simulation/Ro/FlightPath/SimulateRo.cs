using Core.Models;
﻿using Simulation.Models;

namespace Simulation.Ro.FlightPath
{
    public class SimulateRo
    {
        public string Message { get; set; } = string.Empty;
        public Location StartLocation { get; set; }
        public Location DestinationLocation { get; set; }
        public double CruiseAltitude { get; set; }
        public string UavId { get; set; } = string.Empty;
    }
}
