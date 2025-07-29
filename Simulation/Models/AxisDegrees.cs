namespace Simulation.Models
{
    public struct AxisDegrees(double yaw, double pitch, double roll)
    {
        public double Yaw { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }

    }
}
