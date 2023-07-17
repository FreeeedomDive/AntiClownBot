namespace AntiClown.DiscordBot.Interactivity.Repository;

public interface IInteractivityRepository
{
    Task CreateAsync(Domain.Interactivity interactivity);
    Task<Domain.Interactivity> ReadAsync(Guid id);
}