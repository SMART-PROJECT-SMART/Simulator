using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Simulation.Common.constants;
using Simulation.Configuration;
using Simulation.Models.ICDModels;

namespace Simulation.Services.ICDDirectory
{
    public class ICDDirectory : IICDDirectory
    {
        private readonly List<ICD> _icds = new List<ICD>();
        private readonly string _directoryPath;

        public ICDDirectory(IOptions<ICDSettings> opts)
        {
            _directoryPath = !string.IsNullOrWhiteSpace(opts.Value.ICDFilePath)
                ? opts.Value.ICDFilePath
                : throw new DirectoryNotFoundException();
        }

        public void LoadAllICDs()
        {
            var jsonFiles = Directory.GetFiles(
                _directoryPath,
                SimulationConstants.Config.JSON_SEARCH_PATTERN,
                SearchOption.TopDirectoryOnly
            );
            foreach (var filePath in jsonFiles)
            {
                LoadICD(filePath);
            }
        }

        private void LoadICD(string fullFilePath)
        {
            var fileJson = File.ReadAllText(fullFilePath);
            var icdFields = JsonConvert.DeserializeObject<List<ICDItem>>(fileJson);
            var icd = new ICD(icdFields);
            _icds.Add(icd);
        }

        public List<ICD> GetAllICDs()
        {
            return _icds;
        }
    }
}
