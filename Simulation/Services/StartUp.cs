using System.Collections;
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
        private static readonly Random _random = new Random();

        public StartUp(IICDDirectory directory, IICDNetworking networking)
        {
            _directory = directory;
            _networking = networking;
        }

        public void LoadAndSendICD(StartUpLoadDto dto)
        {
            BitArray icdDataBitArray = _directory.DecodeICD(dto.ICDName);
            _networking.SendICDByteArray(dto.Channel, icdDataBitArray);
        }

        public void LoadAndSendICDsOnStartup()
        {
            Console.WriteLine("=== STARTUP: Loading and sending ICD files ===");
            
            int northPort = 8000;
            int southPort = 9000;

            var northChannel = new Channel(1, northPort);
            var southChannel = new Channel(2, southPort);

            var northDto = new StartUpLoadDto("north_telemetry_icd.json", northChannel);
            var southDto = new StartUpLoadDto("south_telemetry_icd.json", southChannel);

            Console.WriteLine($"STARTUP: Sending North ICD to localhost:{northPort}");
            Console.WriteLine($"STARTUP: Sending South ICD to localhost:{southPort}");

            try
            {
                LoadAndSendICD(northDto);
                Console.WriteLine("STARTUP: North ICD sent successfully");
                
                LoadAndSendICD(southDto);
                Console.WriteLine("STARTUP: South ICD sent successfully");
                
                Console.WriteLine("=== STARTUP: All ICD files sent successfully ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"STARTUP ERROR: Failed to send ICD files - {ex.Message}");
                Console.WriteLine($"STARTUP ERROR: Stack trace: {ex.StackTrace}");
            }
        }
    }
}
