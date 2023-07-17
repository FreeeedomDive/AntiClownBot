namespace AntiClown.DiscordBot.Interactivity.Repository;

public interface IInteractivityRepository
{
    Task CreateAsync<T>(Domain.Interactivity<T> interactivity);
    Task UpdateAsync<T>(Domain.Interactivity<T> interactivity);
    Task<Domain.Interactivity<T>?> TryReadAsync<T>(Guid id);
}