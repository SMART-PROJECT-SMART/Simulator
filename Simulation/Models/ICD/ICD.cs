using System.Collections;
using Newtonsoft.Json;

namespace Simulation.Models.ICD
{
    public class ICD : IEnumerable<ICDItem>
    {
        [JsonProperty]
        public List<ICDItem> Document { get; set; } = new List<ICDItem>();

        public ICD() { }

        public ICD(List<ICDItem> document)
        {
            Document = document;
        }

        public IEnumerator<ICDItem> GetEnumerator()
        {
            return Document.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}