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

                string targetHost = GetLocalIPAddress();

                try
                {
                    client.Send(byteArray, byteArray.Length, targetHost, channel.PortNumber);
                }
                catch (Exception ex)
                {
                    client.Send(
                        byteArray,
                        byteArray.Length,
                        SimulationConstants.Networking.HOST,
                        channel.PortNumber
                    );
                }
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect(
                        SimulationConstants.Networking.GOOGLE_DNS_PRIMARY,
                        SimulationConstants.Networking.SOCKET_CONNECT_PORT
                    );
                    IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint?.Address.ToString()
                        ?? SimulationConstants.Networking.FALLBACK_IP;
                }
            }
            catch
            {
                return SimulationConstants.Networking.FALLBACK_IP;
            }
        }
    }
}
