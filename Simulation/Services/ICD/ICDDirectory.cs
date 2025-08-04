using System.Collections;
using Newtonsoft.Json;
using Simulation.Common.Enums;
using Simulation.Models.ICD;
using Simulation.Services.Helpers;

namespace Simulation.Services.ICD
{
    public class ICDDirectory
    {
        private static Models.ICD.ICD DeSerializeICD(string icdName)
        {
            string path = $"SimulationConstants.ICDGeneration.ICD_DIRECTORY/{icdName}";
            string fileJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Models.ICD.ICD>(fileJson);
        }

        public static BitArray DecodeICD(string icdName)
        {
            Dictionary<TelemetryFields,double> telemetryData = new Dictionary<TelemetryFields, double>();
            Models.ICD.ICD icd = DeSerializeICD(icdName);
            foreach (ICDItem item  in icd)
            {
                telemetryData[item.Name] = item.Value;
            }

            return new BitArray(1);
            //TelemetryCompressionHelper.CompressTelemetryData(telemetryData);
        }
    }

}
