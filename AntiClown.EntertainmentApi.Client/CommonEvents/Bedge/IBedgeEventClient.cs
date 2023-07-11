namespace AntiClown.EntertainmentApi.Client.CommonEvents.Bedge;

public interface IBedgeEventClient
{
    Task<Guid> StartNewAsync();
}