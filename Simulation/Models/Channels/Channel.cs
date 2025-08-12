using System.Collections;
using System.Net.Sockets;
using Shared.Models.ICDModels;
using Simulation.Common.constants;
using Simulation.Services.Helpers;

namespace Simulation.Models.Channels
{
    public class Channel
    {
        public int TailId { get; set; }
        public int PortNumber { get; set; }
        public ICD ICD { get; set; }
        public UdpClient UdpClient { get; set; }

        public Channel(int tailId, int portNumber, ICD icd)
        {
            TailId = tailId;
            PortNumber = portNumber;
            ICD = icd;
            UdpClient = new UdpClient();
        }

        public void SendICDByteArray(BitArray data)
        {
            var byteArray = data.ToByteArray();
            UdpClient.Send(
                byteArray,
                byteArray.Length,
                SimulationConstants.Networking.FALLBACK_IP,
                PortNumber
            );
        }
    }
}
