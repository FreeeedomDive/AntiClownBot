using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AntiClown.Core.Serializers;

public class NewtonsoftJsonSerializer : IJsonSerializer
{
    public string Serialize<T>(T obj)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
            },
        };

        return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
    }

    public T Deserialize<T>(string json)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
            },
        };

        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}