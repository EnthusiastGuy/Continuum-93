using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContinuumTools.Network
{
    public static class NOptions
    {
        public static JsonSerializerSettings newtonsoftJsonSerializerSettings = new JsonSerializerSettings
        {
            // Handle missing members gracefully
            MissingMemberHandling = MissingMemberHandling.Ignore,

            // Ignore null values when deserializing
            NullValueHandling = NullValueHandling.Ignore,

            // Allow case-insensitive property matching
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },

            // For debugging: include more detailed error messages
            Error = (sender, args) =>
            {
                Console.WriteLine($"JSON Error: {args.ErrorContext.Error.Message}");
                args.ErrorContext.Handled = true; // Suppress exceptions
            }
        };

        public static JsonSerializerOptions jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
