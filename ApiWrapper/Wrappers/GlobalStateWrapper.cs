using System.Collections.Generic;
using ApiWrapper.Responses.UserCommandResponses;
using Newtonsoft.Json;
using RestSharp;

namespace ApiWrapper.Wrappers
{
    public class GlobalStateWrapper : BaseWrapper
    {
        private static string WrapperUrl => "api/globalState/";

        public static List<TributeResponseDto> GetAutomaticTributes()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"{WrapperUrl}/autoTributes");
            var response = client.Post(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<List<TributeResponseDto>>(content, new ItemConverter());
        }
    }
}