using System.Collections;
using Newtonsoft.Json;
using Simulation.Common.Enums;
using Simulation.Models.ICD;
using Simulation.Services.Helpers;

namespace Simulation.Services.ICD
{
    public class ICDDirectory
    {
        private  Models.ICD.ICD DeSerializeICD(string icdName)
        {
            string path = $"SimulationConstants.ICDGeneration.ICD_DIRECTORY/{icdName}";
            string fileJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Models.ICD.ICD>(fileJson)!;
        }

        public BitArray DecodeICD(string icdName)
        {
            Dictionary<TelemetryFields,double> telemetryData = new Dictionary<TelemetryFields, double>();
            Models.ICD.ICD icd = DeSerializeICD(icdName);

            return TelemetryCompressionHelper.CompressTelemetryData(telemetryData);
        }
    }

}
