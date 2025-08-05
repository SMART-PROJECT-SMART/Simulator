using System.Collections;
using System.Net;
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
                var byteArray = data.ToByteArray();

                string targetHost = SimulationConstants.Networking.FALLBACK_IP;

                client.Send(byteArray, byteArray.Length, targetHost, channel.PortNumber);
            }
        }

    }
}
