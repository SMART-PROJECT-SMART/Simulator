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
    }
}
