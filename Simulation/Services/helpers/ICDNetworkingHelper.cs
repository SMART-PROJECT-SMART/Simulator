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

                client.Send(
                    byteArray,
                    byteArray.Length,
                    SimulationConstants.Networking.FALLBACK_IP,
                    channel.PortNumber
                );
            }
        }
    }
}
