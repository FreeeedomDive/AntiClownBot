using AntiClownApiClient.Dto.Constants;
using AntiClownApiClient.Dto.Models.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace AntiClownApiClient.Utils
{
    public class ItemConverter: JsonConverter<BaseItem>
    {
        public override void WriteJson(JsonWriter writer, BaseItem value, JsonSerializer serializer)
        {
            
        }

        public override BaseItem ReadJson(JsonReader reader, Type objectType, BaseItem existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var id = (Guid) jo["Id"];
            var itemType = (string)jo["Name"];

            BaseItem item = itemType switch
            {
                StringConstants.CatWifeName => new CatWife(id),
                StringConstants.DogWifeName => new DogWife(id),
                StringConstants.RiceBowlName => new RiceBowl(id),
                StringConstants.InternetName => new Internet(id),
                StringConstants.JadeRodName => new JadeRod(id),
                StringConstants.CommunismBannerName => new CommunismBanner(id),
                _ => throw new ArgumentOutOfRangeException(nameof(itemType))
            };

            serializer.Populate(jo.CreateReader(), item);

            return item;
        }
    }
}