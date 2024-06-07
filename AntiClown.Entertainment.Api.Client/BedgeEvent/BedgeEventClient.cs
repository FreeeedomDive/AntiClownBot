/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.BedgeEvent;

public class BedgeEventClient : IBedgeEventClient
{
    public BedgeEventClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/bedge/start", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    private readonly RestSharp.RestClient restClient;
}
