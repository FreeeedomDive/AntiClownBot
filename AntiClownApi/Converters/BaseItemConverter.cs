using System;
using System.Text.Json;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Converters
{
    public class BaseItemConverter: System.Text.Json.Serialization.JsonConverter<BaseItem>
    {
        public override BaseItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, BaseItem value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType());
        }
    }
}