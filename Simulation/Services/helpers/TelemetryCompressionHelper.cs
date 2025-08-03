using System;
using System.Collections.Generic;
using Simulation.Common.Enums;

namespace Simulation.Services.Flight_Path.helpers
{
    public static class TelemetryCompressionHelper
    {
        public static byte[] CompressTelemetryData(Dictionary<TelemetryFields, double> telemetryData)
        {
            var fields = Enum.GetValues<TelemetryFields>();
            byte[] result = new byte[fields.Length * 4];

            for (int i = 0; i < fields.Length; i++)
            {
                TelemetryFields field = fields[i];
                if (telemetryData.TryGetValue(field, out double value))
                {
                    float floatValue = (float)value;
                    byte[] floatBytes = BitConverter.GetBytes(floatValue);

                    int baseIndex = i * 4;
                    result[baseIndex] = floatBytes[0];
                    result[baseIndex + 1] = floatBytes[1];
                    result[baseIndex + 2] = floatBytes[2];
                    result[baseIndex + 3] = floatBytes[3];
                }
            }

            return result;
        }

        public static Dictionary<TelemetryFields, double> DecompressTelemetryData(byte[] compressedData)
        {
            var fields = Enum.GetValues<TelemetryFields>();
            Dictionary<TelemetryFields, double> result = new Dictionary<TelemetryFields, double>();

            for (int i = 0; i < fields.Length && (i * 4 + 3) < compressedData.Length; i++)
            {
                TelemetryFields field = fields[i];

                int baseIndex = i * 4;
                byte[] floatBytes = new byte[4];
                floatBytes[0] = compressedData[baseIndex];
                floatBytes[1] = compressedData[baseIndex + 1];
                floatBytes[2] = compressedData[baseIndex + 2];
                floatBytes[3] = compressedData[baseIndex + 3];

                float floatValue = BitConverter.ToSingle(floatBytes, 0);
                result[field] = (double)floatValue;
            }

            return result;
        }
    }
}