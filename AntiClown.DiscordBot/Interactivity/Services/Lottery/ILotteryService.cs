namespace AntiClown.DiscordBot.Interactivity.Services.Lottery;

public interface ILotteryService
{
    Task StartAsync(Guid eventId);
    Task AddParticipantAsync(Guid eventId, ulong memberId);
    Task FinishAsync(Guid eventId);
}