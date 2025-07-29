namespace Simulation.Models
{
    public struct Location(double latitude, double longitude, double altitude)
    {
        public double Latitude { get; set; } = latitude;
        public double Longitude { get; set; } = longitude;
        public double Altitude { get; set; } = altitude;
    }

}
