using System;
using System.Collections.Generic;
using ApiWrapper.Models.Items;
using ApiWrapper.Responses.UserCommandResponses;
using Newtonsoft.Json;
using RestSharp;

namespace ApiWrapper.Wrappers
{
    public class ItemsApi : BaseApi
    {
        private static string WrapperUrl(ulong userId) => $"api/users/{userId}/items";

        public static List<BaseItem> AllItems(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl(userId)}");
            var response = client.Get(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<List<BaseItem>>(content, new ItemConverter());
        }

        public static SetActiveStatusForItemResponseDto SetActiveStatusForItem(ulong userId, Guid itemId, bool isActive)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl(userId)}/{itemId}/active/{isActive}");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<SetActiveStatusForItemResponseDto>(content);
        }

        public static SellItemResponseDto SellItem(ulong userId, Guid itemId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl(userId)}/{itemId}/sell");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<SellItemResponseDto>(content);
        }

        public static OpenLootBoxResultDto OpenLootBox(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl(userId)}/lootbox/open");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<OpenLootBoxResultDto>(content, new ItemConverter());
        }

        public static string AddLootBox(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl(userId)}/lootbox/add");
            var response = client.Post(request);
            return response.Content;
        }

        public static string RemoveLootBox(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl(userId)}/lootbox/remove");
            var response = client.Post(request);
            return response.Content;
        }
    }
}