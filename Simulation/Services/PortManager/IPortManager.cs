using Simulation.Models.Channels;

namespace Simulation.Services.PortManager
{
    public interface IPortManager
    {
        public bool IsUsed(int portNumber);
        public void SwitchPorts(int sourcePorts, int targetPort);
        public void AssignPort(Channel channel, int portNumber);
        public void RemovePort(int portNumber);
        public void ClearPorts();
        public IEnumerable<int> AllocatePorts(int count);
    }
}
