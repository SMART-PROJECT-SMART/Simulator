namespace Simulation.Models
{
    public struct AxisDegrees(double yaw, double pitch, double roll)
    {
        public double Yaw { get; set; } = yaw;
        public double Pitch { get; set; } = pitch;
        public double Roll { get; set; } = roll;

    }
}
