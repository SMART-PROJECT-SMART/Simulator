using Core.Models;
﻿using Simulation.Models;

namespace Simulation.Dto.FlightPath
{
    public class SimulateDto
    {
        public int TailId { get; set; }
        public Location Destination { get; set; }
        public string MissionId { get; set; } = string.Empty;

        public SimulateDto() { }

        public SimulateDto(int tailId, Location destination, string missionId)
        {
            TailId = tailId;
            Destination = destination;
            MissionId = missionId;
        }
    }
}
