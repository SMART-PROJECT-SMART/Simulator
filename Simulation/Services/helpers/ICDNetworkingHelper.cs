using System.Collections;
using System.Net.Sockets;
using Simulation.Common.constants;
using Simulation.Models.Channels;

namespace Simulation.Services.Helpers
{
    public class ICDNetworkingHelper : IICDNetworking
    {
        public void SendICDByteArray(Channel channel, BitArray data)
        {
            using (var client = new UdpClient())
            {
                client.Send(data.ToByteArray(), data.Length, SimulationConstants.Networking.HOST, channel.PortNumber);
            }
        }
    }
}
