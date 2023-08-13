namespace AntiClown.Entertainment.Api.Client.CommonEvents.Bedge;

public interface IBedgeEventClient
{
    Task<Guid> StartNewAsync();
}