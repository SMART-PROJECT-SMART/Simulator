namespace Simulation.Services.PortManager
{
    public interface IPortManager
    {
        public bool isUsed(int portNumber);
        public void switchPorts(int tailId,int usedPort,int targetPort);
        public void AssignPort(int tailId, int portNumber);
    }
} 