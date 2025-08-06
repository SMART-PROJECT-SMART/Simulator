using System.Collections;
using Simulation.Models.Channels;

namespace Simulation.Services.Helpers.ICDNetworking
{
    public interface IICDNetworking
    {
        void SendICDByteArray(Channel channel, BitArray data);
        void RemoveUAVConnections(int tailId);
        void RemoveChannelConnection(int tailId, int portNumber);
        int GetActiveConnectionCount(int tailId);
        int GetTotalActiveConnections();
        void Dispose();
    }
}