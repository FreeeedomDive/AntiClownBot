namespace AntiClown.Entertainment.Api.Client.DailyEvents.PaymentsAndResets;

public interface IPaymentsAndResetsClient
{
    Task<Guid> StartNewAsync();
}