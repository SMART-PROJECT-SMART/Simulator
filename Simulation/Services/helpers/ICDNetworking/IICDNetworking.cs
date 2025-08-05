using System.Collections;
using Simulation.Models.Channels;

namespace Simulation.Services.Helpers.ICDNetworking
{
    public interface IICDNetworking
    {
        public void SendICDByteArray(Channel channel, BitArray data);
    }
}
