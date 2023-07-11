using AntiClown.EntertainmentApi.Client.Extensions;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Bedge
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

        private const string ControllerUrl = "events/common/bedge";
        private readonly RestClient restClient;
    }
}

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Bedge
{
}