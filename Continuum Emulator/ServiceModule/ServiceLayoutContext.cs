using System.Text.Json.Serialization;

namespace Continuum93.ServiceModule
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ServiceLayoutState))]
    [JsonSerializable(typeof(WindowLayoutDto))]
    internal partial class ServiceLayoutContext : JsonSerializerContext
    {
    }
}

