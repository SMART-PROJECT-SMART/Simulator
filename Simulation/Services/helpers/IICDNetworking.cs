using System.Collections;
using Simulation.Models.Channels;

namespace Simulation.Services.Helpers
{
    public interface IICDNetworking
    {
        public void SendICDByteArray(Channel channel, BitArray data);
    }
}
