using System;
using System.Collections.Generic;
using ApiWrapper.Requests;
using ApiWrapper.Responses.UserCommandResponses;
using Newtonsoft.Json;
using RestSharp;

namespace ApiWrapper.Wrappers
{
    public class UsersApi : BaseApi
    {
        private static string WrapperUrl => "api/users/";

        public static TributeResponseDto Tribute(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/tribute");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<TributeResponseDto>(content);
        }

        public static WhenNextTributeResponseDto WhenNextTribute(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/tribute/when");
            var response = client.Get(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<WhenNextTributeResponseDto>(content);
        }

        public static RatingResponseDto Rating(ulong userId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/rating");
            var response = client.Get(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<RatingResponseDto>(content, new ItemConverter());
        }

        public static ChangeUserBalanceResponseDto ChangeUserRating(ulong userId, int ratingDiff, string reason)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}changeBalance");
            request.AddJsonBody(new ChangeUserRatingRequestDto
            {
                UserId = userId,
                RatingDiff = ratingDiff,
                Reason = reason
            });
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<ChangeUserBalanceResponseDto>(content);
        }
        
        public static BulkChangeUserBalanceResponseDto BulkChangeUserBalance(List<ulong> users, int ratingDiff, string reason)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}bulkChangeBalance");
            request.AddJsonBody(new BulkChangeUserBalanceRequestDto
            {
                Users = users,
                RatingDiff = ratingDiff,
                Reason = reason
            });
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<BulkChangeUserBalanceResponseDto>(content);
        }

        public static AllUsersResponseDto GetAllUsers()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}");
            var response = client.Get(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<AllUsersResponseDto>(content);
        }

        public static void RemoveCooldowns()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}removeCooldowns");
            client.Post(request);
        }

        public static ulong GetRichestUser()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}richest");
            var response = client.Get(request);
            return ulong.Parse(response.Content);
        }

        public static ItemResponseDto GetItemById(ulong userId, Guid itemId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}{userId}/items/{itemId}");
            var response = client.Get(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<ItemResponseDto>(content, new ItemConverter());
        }

        public static void DailyReset()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}dailyReset");
            client.Post(request);
        }
    }
}