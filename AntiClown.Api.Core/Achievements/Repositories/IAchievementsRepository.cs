using AntiClown.Api.Core.Achievements.Domain;

namespace AntiClown.Api.Core.Achievements.Repositories;

public interface IAchievementsRepository
{
    Task<Achievement[]> ReadAllAsync();
    Task<Achievement> ReadAsync(Guid id);
    Task CreateAsync(Achievement achievement);
    Task DeleteAsync(Guid id);
}

