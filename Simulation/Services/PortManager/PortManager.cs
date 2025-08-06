using System;
using System.Collections.Concurrent;
using Simulation.Common.constants;

namespace Simulation.Services.PortManager
{
    public class PortManager : IPortManager
    {
       private readonly Dictionary<int,int> _portUavs = new Dictionary<int,int>();

       public bool isUsed(int portNumber)
       {
           return !_portUavs.ContainsKey(portNumber);
       }

       public void switchPorts(int tailId, int usedPort, int targetPort)
       {
           if (!isUsed(usedPort))
           {
                _portUavs[targetPort] = tailId;
                _portUavs.Remove(usedPort);
           }
           else
           {
               int prevPortTailId = _portUavs[targetPort];
               _portUavs[targetPort] = tailId;
               _portUavs[usedPort] = prevPortTailId;
           }
       }

       public void AssignPort(int tailId, int portNumber)
       {
           _portUavs.TryAdd(portNumber, tailId);
       }
    }
}
