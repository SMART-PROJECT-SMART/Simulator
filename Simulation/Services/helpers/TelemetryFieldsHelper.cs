using Simulation.Common.Enums;
using Simulation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simulation.Services.helpers
{
    public static class TelemetryFieldsHelper
    {
        public static Dictionary<TelemetryFields, double> Initialize(params string[] categories)
        {
            var telemetryData = new Dictionary<TelemetryFields, double>();

            foreach (TelemetryFields field in Enum.GetValues<TelemetryFields>())
            {
                var attribute = GetTelemetryCategoryAttribute(field);
                if (attribute != null && categories.Contains(attribute.Category))
                {
                    telemetryData[field] = GetDefaultValue(field);
                }
            }

            return telemetryData;
        }

        public static Dictionary<TelemetryFields, double> FlightOnly()
        {
            return Initialize(TelemetryCategories.Flight);
        }

        public static void SetLocation(this Dictionary<TelemetryFields, double> telemetryData, Location location)
        {
            telemetryData[TelemetryFields.Latitude] = location.Latitude;
            telemetryData[TelemetryFields.Longitude] = location.Longitude;
            telemetryData[TelemetryFields.Altitude] = location.Altitude;
        }

        private static TelemetryCategoryAttribute GetTelemetryCategoryAttribute(TelemetryFields field)
        {
            var fieldInfo = typeof(TelemetryFields).GetField(field.ToString());
            return fieldInfo?.GetCustomAttribute<TelemetryCategoryAttribute>();
        }

        private static double GetDefaultValue(TelemetryFields field)
        {
            return field switch
            {

                TelemetryFields.DataStorageUsedGB => 0.0,

                _ => 0.0
            };
        }
    }
}