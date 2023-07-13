namespace AntiClown.EntertainmentApi.Client.DailyEvents.PaymentsAndResets;

public interface IPaymentsAndResetsClient
{
    Task<Guid> StartNewAsync();
}