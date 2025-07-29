using System.Reflection;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Common.constants;

namespace Simulation.Services.helpers
{
    public static class TelemetryFieldsHelper
    {
        public static Dictionary<TelemetryFields, double> Initialize(params string[] categories)
        {
            var dict = new Dictionary<TelemetryFields, double>();
            var all = Enum.GetValues(typeof(TelemetryFields))
                          .Cast<TelemetryFields>();
            foreach (var field in all)
            {
                var catAttr = typeof(TelemetryFields)
                    .GetField(field.ToString())!
                    .GetCustomAttributes(typeof(TelemetryCategoryAttribute), false)
                    .Cast<TelemetryCategoryAttribute>()
                    .FirstOrDefault();
                if (categories.Length == 0 || categories.Contains(catAttr?.Category))
                    dict[field] = 0.0;
            }
            return dict;
        }

        public static Dictionary<TelemetryFields, double> FlightOnly()  
            => Initialize(TelemetryCategories.Flight);
        
        public static Dictionary<TelemetryFields, double> ArmedOnly()   
            => Initialize(TelemetryCategories.Armed);
        
        public static Dictionary<TelemetryFields, double> SurveillanceOnly() 
            => Initialize(TelemetryCategories.Surveillance);
        
        public static Dictionary<TelemetryFields, double> All()        
            => Initialize(); 

        public static void SetLocation(this Dictionary<TelemetryFields, double> telemetry, Location loc)
        {
            telemetry[TelemetryFields.Latitude] = loc.Latitude;
            telemetry[TelemetryFields.Longitude] = loc.Longitude;
            telemetry[TelemetryFields.Altitude] = loc.Altitude;
            telemetry[TelemetryFields.CurrentSpeedKmph] = SimulationConstants.FlightPath.MIN_SPEED_KMH;
        }
    }
} 