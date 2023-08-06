namespace AntiClown.DiscordBot.Interactivity.Services.Race;

public interface IRaceService
{
    Task StartAsync(Guid eventId);
    Task AddParticipantAsync(Guid eventId, ulong memberId);
    Task FinishAsync(Guid eventId);
}