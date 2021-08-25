using System;
using ApiWrapper.Responses.ShopResponses;
using ApiWrapper.Responses.UserCommandResponses;
using Newtonsoft.Json;
using RestSharp;

namespace ApiWrapper.Wrappers
{
    public class ShopWrapper: BaseWrapper
    {
        private static string WrapperUrl => "api/shop/";

        public static UserShopResponseDto UserShop(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}");
            var response = client.Get(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<UserShopResponseDto>(content);
        }

        public static ItemIdInSlotResponseDto ItemIdInSlot(ulong userId, int slot)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/getItemIdInSlot/{slot}");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<ItemIdInSlotResponseDto>(content);
        }

        public static ShopItemRevealResponseDto ItemReveal(ulong userId, Guid itemId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/reveal/{itemId}");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<ShopItemRevealResponseDto>(content);
        }

        public static BuyItemResponseDto Buy(ulong userId, Guid itemId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/buy/{itemId}");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<BuyItemResponseDto>(content);
        }
        
        public static ReRollResponseDto ReRoll(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/reroll");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<ReRollResponseDto>(content);
        }
    }
}