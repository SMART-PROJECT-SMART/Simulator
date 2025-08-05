using Simulation.Common.constants;

namespace Simulation.Services.PortManager
{
    public class PortManager : IPortManager
    {
        private readonly Dictionary<int, HashSet<int>> _usedPorts = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, int> _nextPortNumbers = new Dictionary<int, int>();

        public int GetNextAvailablePort(int tailId)
        {
            if (!_nextPortNumbers.ContainsKey(tailId))
            {
                _nextPortNumbers[tailId] = SimulationConstants.Networking.STARTING_PORT_NUMBER;
            }

            int portNumber = _nextPortNumbers[tailId];
            
            while (_usedPorts.ContainsKey(tailId) && _usedPorts[tailId].Contains(portNumber))
            {
                portNumber += SimulationConstants.Networking.PORT_INCREMENT;
                if (portNumber > SimulationConstants.Networking.MAX_PORT_NUMBER)
                {
                    portNumber = SimulationConstants.Networking.STARTING_PORT_NUMBER;
                }
            }

            _nextPortNumbers[tailId] = portNumber + SimulationConstants.Networking.PORT_INCREMENT;
            return portNumber;
        }

        public void ReservePort(int tailId, int portNumber)
        {
            if (!_usedPorts.ContainsKey(tailId))
            {
                _usedPorts[tailId] = new HashSet<int>();
            }
            _usedPorts[tailId].Add(portNumber);
        }

        public void ReleasePort(int tailId, int portNumber)
        {
            if (_usedPorts.ContainsKey(tailId))
            {
                _usedPorts[tailId].Remove(portNumber);
            }
        }


    }
} 