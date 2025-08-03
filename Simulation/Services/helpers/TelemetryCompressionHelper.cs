using Simulation.Common.constants;
using Simulation.Common.Enums;

namespace Simulation.Services.Helpers
{
    public static class TelemetryCompressionHelper
    {
        private static readonly Dictionary<TelemetryFields, int> _sizeInBytes = new()
        {
            { TelemetryFields.DragCoefficient, 2 },         
            { TelemetryFields.LiftCoefficient, 2 },         
            { TelemetryFields.ThrottlePercent, 1 },         
            { TelemetryFields.CruiseAltitude, 4 },          
            { TelemetryFields.Latitude, 4 },                
            { TelemetryFields.LandingGearStatus, 1 },       
            { TelemetryFields.Longitude, 4 },               
            { TelemetryFields.Altitude, 4 },                
            { TelemetryFields.CurrentSpeedKmph, 2 },        
            { TelemetryFields.YawDeg, 2 },                  
            { TelemetryFields.PitchDeg, 2 },                
            { TelemetryFields.RollDeg, 2 },                 
            { TelemetryFields.ThrustAfterInfluence, 2 },    
            { TelemetryFields.FuelAmount, 2 },              
            { TelemetryFields.DataStorageUsedGB, 2 },       
            { TelemetryFields.FlightTimeSec, 4 },           
            { TelemetryFields.SignalStrength, 2 },          
            { TelemetryFields.Rpm, 2 },                     
            { TelemetryFields.EngineDegrees, 2 }            
        };

        public static byte[] CompressTelemetryData(Dictionary<TelemetryFields, double> telemetryData)
        {
            int totalSize = _sizeInBytes.Values.Sum();
            byte[] result = new byte[totalSize];
            int offset = 0;

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                double value = telemetryData.GetValueOrDefault(field, 0.0);
                int size = _sizeInBytes[field];

                byte[] fieldBytes = size switch
                {
                    1 => new[] { (byte)value },
                    2 => BitConverter.GetBytes((short)value),
                    4 => BitConverter.GetBytes((float)value),
                    8 => BitConverter.GetBytes(value),
                    _ => throw new NotSupportedException()
                };

                Buffer.BlockCopy(fieldBytes, 0, result, offset, size);
                offset += size;
            }

            return result;
        }

        public static Dictionary<TelemetryFields, double> DecompressTelemetryData(byte[] compressedData)
        {
            Dictionary<TelemetryFields, double> result = new();
            int offset = 0;

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                int size = _sizeInBytes[field];
                double value = size switch
                {
                    1 => compressedData[offset],
                    2 => BitConverter.ToInt16(compressedData, offset),
                    4 => BitConverter.ToSingle(compressedData, offset),
                    8 => BitConverter.ToDouble(compressedData, offset),
                    _ => throw new NotSupportedException()
                };

                result[field] = value;
                offset += size;
            }

            return result;
        }
    }
}