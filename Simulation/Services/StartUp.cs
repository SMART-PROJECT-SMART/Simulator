using System.Collections;
using Simulation.Dto.ICD;
using Simulation.Services.Helpers;
using Simulation.Services.ICD;

namespace Simulation.Services
{
    public class StartUp
    {
        private readonly IICDDirectory _directory;
        private readonly IICDNetworking _networking;

        public StartUp(IICDDirectory directory, IICDNetworking networking)
        {
            _directory = directory;
            _networking = networking;
        }


        public void LoadAndSendICD(StartUpLoadDto dto)
        {
            BitArray icdDataBitArray = _directory.DecodeICD(dto.ICDName);
            _networking.SendICDByteArray(dto.Channel,icdDataBitArray);
        }
    }
}
