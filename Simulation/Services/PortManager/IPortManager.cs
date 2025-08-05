namespace Simulation.Services.PortManager
{
    public interface IPortManager
    {
        int GetNextAvailablePort(int tailId);
        void ReservePort(int tailId, int portNumber);
        void ReleasePort(int tailId, int portNumber);
    }
} 