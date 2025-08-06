using System.Collections;
using Simulation.Models.Channels;
using Simulation.Models.ICDModels;
using Simulation.Services.PortManager;

namespace Simulation.Services
{
    public class ChannelManager : IDisposable
    {
        private readonly Dictionary<int, List<Channel>> _uavChannels = new Dictionary<int, List<Channel>>();
        private readonly IPortManager _portManager;
        private bool _disposed = false;

        public ChannelManager(IPortManager portManager)
        {
            _portManager = portManager;
        }

        public void AddChannel(int tailId, Channel channel)
        {
            if (!_uavChannels.ContainsKey(tailId))
            {
                _uavChannels[tailId] = new List<Channel>();
            }
            _uavChannels[tailId].Add(channel);
        }
        public void SendCompressedToChannel(int tailId, ICD icd, BitArray compressedData)
        {
            if (_uavChannels.TryGetValue(tailId, out var channels))
            {
                var channel = channels.FirstOrDefault(c => c.ICD == icd);
                if (channel != null)
                {

                }
            }
        }

        public List<Channel> GetChannels(int tailId)
        {
            return _uavChannels.TryGetValue(tailId, out var channels) ? channels : new List<Channel>();
        }

        public void Dispose()
        {
        }
    }
}