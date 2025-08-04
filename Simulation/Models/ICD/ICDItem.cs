using Simulation.Common.Enums;

namespace Simulation.Models.ICD
{
    public class ICDItem
    {
        public TelemetryFields Name { get; set; }
        public object Type { get; set; }
        public Type Unit { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int StartBitArrayIndex { get; set; }
        public int BitLength { get; set; }

        public ICDItem(TelemetryFields name, object type, Type unit, double minValue, double maxValue, int startBitArrayIndex, int bitLength)
        {
            Name = name;
            Type = type;
            Unit = unit;
            MinValue = minValue;
            MaxValue = maxValue;
            StartBitArrayIndex = startBitArrayIndex;
            BitLength = bitLength;
        }
    }
}
