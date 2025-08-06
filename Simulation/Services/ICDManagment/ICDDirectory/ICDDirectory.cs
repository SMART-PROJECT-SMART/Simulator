using System.Collections;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Configuration;
using Simulation.Models.ICDModels;
using Simulation.Services.Helpers;

namespace Simulation.Services.ICDManagment.ICDDirectory
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
            var jsonFiles = Directory.GetFiles(_directoryPath, "*.json", SearchOption.TopDirectoryOnly);
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