using System.Collections.Generic;
using System.Net;
using ApiWrapper.Responses.UserCommandResponses;
using Newtonsoft.Json;
using RestSharp;

namespace ApiWrapper.Wrappers
{
    public class GlobalStateApi : BaseApi
    {
        private static string WrapperUrl => "api/globalState/";

        public static List<TributeResponseDto> GetAutomaticTributes()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}autoTributes");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<List<TributeResponseDto>>(content, new ItemConverter());
        }

        public static bool IsBackendWorking()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}ping");
            var response = client.Get(request);
            var result = response.Content;
            return response.IsSuccessful && response.StatusCode == HttpStatusCode.OK;
        }
    }
}