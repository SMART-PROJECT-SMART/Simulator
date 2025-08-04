using System.Net.Sockets;
using Simulation.Common.constants;
using Simulation.Models.Channels;

namespace Simulation.Services.Helpers
{
    public class ICDNetworkingHelper
    {
        public void SendICDByteArray(Channel channel, Byte[] data)
        {
            using (var client = new UdpClient())
            {
                client.Send(data, data.Length, SimulationConstants.Networking.HOST, channel.PortNumber);
            }
        }
    }
}
