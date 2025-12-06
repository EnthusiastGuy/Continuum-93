using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Continuum93.ServiceModule.Themes
{
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(ColorThemeData))]
    public partial class ColorThemeJsonContext : JsonSerializerContext
    {
        
    }
}
