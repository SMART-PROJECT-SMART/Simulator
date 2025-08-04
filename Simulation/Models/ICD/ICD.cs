using System.Collections;

namespace Simulation.Models.ICD
{
    public class ICD(List<ICDItem> document) : IEnumerable<ICDItem>
    {
        public List<ICDItem> Document { get; set; } = document;

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
