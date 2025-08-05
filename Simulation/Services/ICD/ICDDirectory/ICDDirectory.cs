using System.Collections;
using Newtonsoft.Json;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models.ICD;
using Simulation.Services.Helpers;

namespace Simulation.Services.ICD.ICDDirectory
{
    public class ICDDirectory : IICDDirectory
    {
        private Models.ICD.ICD DeSerializeICD(string icdName)
        {
            string path = Path.Combine(SimulationConstants.ICDGeneration.ICD_DIRECTORY, icdName);
            string fileJson = File.ReadAllText(path);

            var jsonObject = JsonConvert.DeserializeAnonymousType(
                fileJson,
                new { telemetryFields = new List<ICDItem>() }
            );

            return new Models.ICD.ICD(jsonObject.telemetryFields);
        }

        public BitArray DecodeICD(string icdName)
        {
            Models.ICD.ICD icd = DeSerializeICD(icdName);
            return TelemetryCompressionHelper.CompressTelemetryData(icd);
        }
    }
}
