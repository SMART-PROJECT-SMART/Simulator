using System;
using System.Collections.Generic;
using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Flight_Path.helpers
{
    public static class TelemetryCompressionHelper
    {
        private static readonly int TOTAL_FIELDS = Enum.GetValues<TelemetryFields>().Length;

        public static byte[] CompressTelemetryData(Dictionary<TelemetryFields, double> telemetryData)
        {
            byte[] result = new byte[TOTAL_FIELDS * SimulationConstants.TelemetryData.BYTES_PER_FIELD];

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                double value = telemetryData.GetValueOrDefault(field, 0.0);
                byte[] doubleBytes = BitConverter.GetBytes(value);

                int baseIndex = (int)field * SimulationConstants.TelemetryData.BYTES_PER_FIELD;
                Buffer.BlockCopy(doubleBytes, 0, result, baseIndex, SimulationConstants.TelemetryData.BYTES_PER_FIELD);
            }

            return result;
        }

        public static Dictionary<TelemetryFields, double> DecompressTelemetryData(byte[] compressedData)
        {
            Dictionary<TelemetryFields, double> result = new Dictionary<TelemetryFields, double>();

            for (int i = 0; i < TOTAL_FIELDS; i++)
            {
                int baseIndex = i * SimulationConstants.TelemetryData.BYTES_PER_FIELD;

                if (baseIndex + SimulationConstants.TelemetryData.BYTES_PER_FIELD - 1 < compressedData.Length)
                {
                    result[(TelemetryFields)i] = BitConverter.ToDouble(compressedData, baseIndex);
                }
            }
            return result;
        }
    }
}