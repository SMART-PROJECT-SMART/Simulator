using Simulation.Common.Enums;

namespace Simulation.Models.ICD
{
    public class ICDItem
    {
        public TelemetryFields Name { get; set; }
        public object Type { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int StartBitArrayIndex { get; set; }
        public int BitLength { get; set; }

        public ICDItem(
            TelemetryFields name,
            object type,
            double value,
            string unit,
            double minValue,
            double maxValue,
            int startBitArrayIndex,
            int bitLength
        )
        {
            Name = name;
            Type = type;
            Value = value;
            Unit = unit;
            MinValue = minValue;
            MaxValue = maxValue;
            StartBitArrayIndex = startBitArrayIndex;
            BitLength = bitLength;
        }
    }
}
