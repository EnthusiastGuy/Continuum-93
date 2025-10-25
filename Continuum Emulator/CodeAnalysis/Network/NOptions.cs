using System.Text.Json.Serialization.Metadata;
using System.Text.Json;

namespace Continuum93.CodeAnalysis.Network
{
    public static class NOptions
    {
        public static JsonSerializerOptions jsonSerializerOptions = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
    }
}
