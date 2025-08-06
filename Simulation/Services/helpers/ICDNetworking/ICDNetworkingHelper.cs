using Simulation.Common.constants;
using Simulation.Models.Channels;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Simulation.Services.Helpers.ICDNetworking
{
    public class ICDNetworkingHelper : IICDNetworking, IDisposable
    {
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, UdpClient>> _uavUdpClients;
        private readonly object _lock = new object();
        private bool _disposed = false;

        public ICDNetworkingHelper()
        {
            _uavUdpClients = new ConcurrentDictionary<int, ConcurrentDictionary<int, UdpClient>>();
        }

        public void SendICDByteArray(Channel channel, BitArray data)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ICDNetworkingHelper));

            var udpClient = GetOrCreateUdpClient(channel.TailId, channel.PortNumber);
            var byteArray = data.ToByteArray();

            try
            {
                udpClient.Send(
                    byteArray,
                    byteArray.Length,
                    SimulationConstants.Networking.FALLBACK_IP,
                    channel.PortNumber
                );
            }
            catch (ObjectDisposedException)
            {
                RemoveUdpClient(channel.TailId, channel.PortNumber);
                udpClient = GetOrCreateUdpClient(channel.TailId, channel.PortNumber);
                udpClient.Send(
                    byteArray,
                    byteArray.Length,
                    SimulationConstants.Networking.FALLBACK_IP,
                    channel.PortNumber
                );
            }
        }

        private UdpClient GetOrCreateUdpClient(int tailId, int portNumber)
        {
            var uavClients = _uavUdpClients.GetOrAdd(tailId, _ => new ConcurrentDictionary<int, UdpClient>());

            return uavClients.GetOrAdd(portNumber, port =>
            {
                try
                {
                    return new UdpClient();
                }
                catch (SocketException ex)
                {
                    throw new InvalidOperationException($"Failed to create UDP client for TailId {tailId}, Port {port}", ex);
                }
            });
        }

        private void RemoveUdpClient(int tailId, int portNumber)
        {
            if (_uavUdpClients.TryGetValue(tailId, out var uavClients))
            {
                if (uavClients.TryRemove(portNumber, out var client))
                {
                    client?.Dispose();
                }
            }
        }

        public void RemoveUAVConnections(int tailId)
        {
            if (_uavUdpClients.TryRemove(tailId, out var uavClients))
            {
                foreach (var kvp in uavClients)
                {
                    kvp.Value?.Dispose();
                }
                uavClients.Clear();
            }
        }

        public void RemoveChannelConnection(int tailId, int portNumber)
        {
            RemoveUdpClient(tailId, portNumber);

            if (_uavUdpClients.TryGetValue(tailId, out var uavClients) && uavClients.IsEmpty)
            {
                _uavUdpClients.TryRemove(tailId, out _);
            }
        }

        public int GetActiveConnectionCount(int tailId)
        {
            return _uavUdpClients.TryGetValue(tailId, out var uavClients) ? uavClients.Count : 0;
        }

        public int GetTotalActiveConnections()
        {
            return _uavUdpClients.Values.Sum(clients => clients.Count);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            lock (_lock)
            {
                if (_disposed)
                    return;

                foreach (var uavClients in _uavUdpClients.Values)
                {
                    foreach (var client in uavClients.Values)
                    {
                        client?.Dispose();
                    }
                    uavClients.Clear();
                }
                _uavUdpClients.Clear();

                _disposed = true;
            }
        }
    }
}