namespace Simulation.Models.Channels
{
    public abstract class Channel
    {
        public int TailId { get; set; }
        public int PortNumber { get; set; }

        protected Channel(int tailId, int portNumber)
        {
            TailId = tailId;
            PortNumber = portNumber;
        }
    }
}
