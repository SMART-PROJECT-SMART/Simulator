using System.Collections;
using Simulation.Models.Channels;
using Simulation.Services.Helpers.ICDNetworking;
using Simulation.Services.PortManager;

namespace Simulation.Services
{
    public class ChannelManager
    {
        private readonly Dictionary<int, List<Channel>> _uavChannels = new Dictionary<int, List<Channel>>();
        private readonly IICDNetworking _icdNetworking;
        private readonly IPortManager _portManager;

        public ChannelManager(IICDNetworking icdNetworking, IPortManager portManager)
        {
            _icdNetworking = icdNetworking;
            _portManager = portManager;
        }

        public void AddChannel(int tailId, Channel channel)
        {
            if (channel.PortNumber == 0)
            {
                channel.PortNumber = _portManager.GetNextAvailablePort(tailId);
            }

            _portManager.ReservePort(tailId, channel.PortNumber);
            
            if (!_uavChannels.ContainsKey(tailId))
            {
                _uavChannels[tailId] = new List<Channel>();
            }
            _uavChannels[tailId].Add(channel);
        }

        public void RemoveChannel(int tailId, Channel channel)
        {
            if (_uavChannels.ContainsKey(tailId))
            {
                _portManager.ReleasePort(tailId, channel.PortNumber);
                _uavChannels[tailId].Remove(channel);
                if (_uavChannels[tailId].Count == 0)
                {
                    _uavChannels.Remove(tailId);
                }
            }
        }

        public void RemoveAllChannels(int tailId)
        {
            if (_uavChannels.ContainsKey(tailId))
            {
                foreach (var channel in _uavChannels[tailId])
                {
                    _portManager.ReleasePort(tailId, channel.PortNumber);
                }
            }
            
            _uavChannels.Remove(tailId);
        }

        public void SendCompressed(int tailId, BitArray compressedData)
        {
            if (_uavChannels.TryGetValue(tailId, out var channels))
            {
                foreach (var channel in channels)
                {
                    _icdNetworking.SendICDByteArray(channel, compressedData);
                }
            }
        }

        public void SendCompressedToChannel(int tailId, Simulation.Models.ICDModels.ICD icd, BitArray compressedData)
        {
            if (_uavChannels.TryGetValue(tailId, out var channels))
            {
                var channel = channels.FirstOrDefault(c => c.ICD == icd);
                if (channel != null)
                {
                    _icdNetworking.SendICDByteArray(channel, compressedData);
                }
            }
        }

        public List<Channel> GetChannels(int tailId)
        {
            return _uavChannels.TryGetValue(tailId, out var channels) ? channels : new List<Channel>();
        }




    }
}
