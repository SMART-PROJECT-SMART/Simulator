using Simulation.Common.constants;
using Simulation.Models.Channels;

namespace Simulation.Services.PortManager
{
    public class PortManager : IPortManager
    {
        private readonly Dictionary<int, Channel> _portChannels = new Dictionary<int, Channel>();

        public bool IsUsed(int portNumber)
        {
            return _portChannels.ContainsKey(portNumber);
        }

        public void SwitchPorts(int sourcePort, int targetPort)
        {
            if (sourcePort == targetPort)
            {
                return;
            }

            _portChannels.TryGetValue(sourcePort, out var sourceChannel);
            bool targetExists = _portChannels.TryGetValue(targetPort, out var targetChannel);

            if (targetExists)
            {
                _portChannels[sourcePort] = targetChannel;
                _portChannels[targetPort] = sourceChannel;

                SwitchPortNumbers(sourceChannel, targetChannel);
            }
            else
            {
                sourceChannel.PortNumber = targetPort;
                _portChannels[targetPort] = sourceChannel;
                _portChannels.Remove(sourcePort);
            }
        }

        public void ClearPorts()
        {
            _portChannels.Clear();
        }

        public void AssignPort(Channel channel, int portNumber)
        {
            channel.PortNumber = portNumber;
            _portChannels[portNumber] = channel;
        }

        public void RemovePort(int portNumber)
        {
            _portChannels.Remove(portNumber, out var _);
        }

        private void SwitchPortNumbers(Channel channel1, Channel channel2)
        {
            (channel1.PortNumber, channel2.PortNumber) = (channel2.PortNumber, channel1.PortNumber);
        }

        public IEnumerable<int> AllocatePorts(int count)
        {
            int totalAvailablePorts =
                SimulationConstants.Networking.MAX_PORT_NUMBER
                - SimulationConstants.Networking.STARTING_PORT_NUMBER
                + 1;
            int availablePorts = totalAvailablePorts - _portChannels.Count;

            if (count > availablePorts)
            {
                throw new InvalidOperationException(
                    $"Cannot allocate {count} ports. Only {availablePorts} ports available."
                );
            }

            List<int> allocatedPorts = new List<int>(count);

            for (
                int portNumber = SimulationConstants.Networking.STARTING_PORT_NUMBER;
                ShouldContinueAllocating(portNumber, allocatedPorts.Count, count);
                portNumber++
            )
            {
                if (!_portChannels.ContainsKey(portNumber))
                {
                    allocatedPorts.Add(portNumber);
                }
            }

            return allocatedPorts;
        }

        private bool ShouldContinueAllocating(int portNumber, int allocatedCount, int requiredCount)
        {
            return portNumber <= SimulationConstants.Networking.MAX_PORT_NUMBER
                && allocatedCount < requiredCount;
        }
    }
}
