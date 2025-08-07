using Simulation.Models.Channels;

namespace Simulation.Services.PortManager
{
    public interface IPortManager
    {
        public bool isUsed(int portNumber);
        public void switchPorts(int sourcePorts, int targetPort);
        public void AssignPort(Channel channel, int portNumber);
        public void RemovePort(int portNumber);
        public void ClearPorts();
    }
} 