namespace Simulation.Models
{
    public class ICDItem
    {
        public string Name { get; set; }
        public Object Type { get; set; }
        public double Value { get; set; }
        public Type Unit { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int StartBitArrayIndex { get; set; }
        public int BitLength { get; set; }

        public ICDItem(string name, object type, double value, Type unit, double minValue, double maxValue, int startBitArrayIndex, int bitLength)
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
