using System.Collections;
using Simulation.Common.constants;
using Simulation.Dto.ICD;
using Simulation.Models.Channels;
using Simulation.Services.Helpers;
using Simulation.Services.ICD;

namespace Simulation.Services
{
    public class StartUp
    {
        private readonly IICDDirectory _directory;
        private readonly IICDNetworking _networking;
        private readonly ICDGenerator _icdGenerator;

        public StartUp(
            IICDDirectory directory,
            IICDNetworking networking,
            ICDGenerator icdGenerator
        )
        {
            _directory = directory;
            _networking = networking;
            _icdGenerator = icdGenerator;
        }

        public void LoadAndSendICD(StartUpLoadDto dto)
        {
            BitArray icdDataBitArray = _directory.DecodeICD(dto.ICDName);
            _networking.SendICDByteArray(dto.Channel, icdDataBitArray);
        }

        public async Task LoadAndSendICDsOnStartup()
        {
            await GenerateICDFilesIfNotExist();

            int northPort = 8000;
            int southPort = 9000;

            var northChannel = new Channel(1, northPort);
            var southChannel = new Channel(2, southPort);

            var northDto = new StartUpLoadDto("north_telemetry_icd.json", northChannel);
            var southDto = new StartUpLoadDto("south_telemetry_icd.json", southChannel);

            LoadAndSendICD(northDto);
            LoadAndSendICD(southDto);
        }

        private async Task GenerateICDFilesIfNotExist()
        {
            string northFilePath = Path.Combine(
                SimulationConstants.ICDGeneration.ICD_DIRECTORY,
                "north_telemetry_icd.json"
            );
            string southFilePath = Path.Combine(
                SimulationConstants.ICDGeneration.ICD_DIRECTORY,
                "south_telemetry_icd.json"
            );

            if (!File.Exists(northFilePath) || !File.Exists(southFilePath))
            {
                await _icdGenerator.GenerateTwoICDDocuments();
            }
        }
    }
}
