namespace Simulation.Models.Channels
{
    public class Channel
    {
        public int TailId { get; set; }
        public int PortNumber { get; set; }
        public ICD ICD { get; set; }

        public Channel(int tailId, int portNumber)
        {
            TailId = tailId;
            PortNumber = portNumber;
        }
    }
}
