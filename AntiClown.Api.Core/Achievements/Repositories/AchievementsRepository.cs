using AntiClown.Api.Core.Achievements.Domain;
using AntiClown.Core.Dto.Exceptions;
using AutoMapper;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Achievements.Repositories;

public class AchievementsRepository(
    ISqlRepository<AchievementStorageElement> sqlRepository,
    IMapper mapper
) : IAchievementsRepository
{
    public async Task<Achievement[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return mapper.Map<Achievement[]>(result);
    }

    public async Task<Achievement> ReadAsync(Guid id)
    {
        try
        {
            var result = await sqlRepository.ReadAsync(id);
            return mapper.Map<Achievement>(result);
        }
        catch (SqlEntityNotFoundException)
        {
            throw new EntityNotFoundException(id);
        }
    }

    public async Task CreateAsync(Achievement achievement)
    {
        var storageElement = mapper.Map<AchievementStorageElement>(achievement);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task DeleteAsync(Guid id)
    {
        await sqlRepository.DeleteAsync(id);
    }
}
