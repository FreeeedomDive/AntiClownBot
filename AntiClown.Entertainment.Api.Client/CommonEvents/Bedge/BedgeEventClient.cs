using AntiClown.Entertainment.Api.Client.Extensions;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Bedge
{
    public class BedgeEventClient : IBedgeEventClient
    {
        public BedgeEventClient(RestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<Guid> StartNewAsync()
        {
            var request = new RestRequest($"{ControllerUrl}/start");
            var response = await restClient.ExecutePostAsync(request);
            return response.TryDeserialize<Guid>();
        }

        private readonly RestClient restClient;

        private const string ControllerUrl = "events/common/bedge";
    }
}

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Bedge
{
}