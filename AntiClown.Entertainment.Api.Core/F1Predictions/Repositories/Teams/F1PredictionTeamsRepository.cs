using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Teams;

public class F1PredictionTeamsRepository : IF1PredictionTeamsRepository
{
    public F1PredictionTeamsRepository(ISqlRepository<F1PredictionTeamStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<F1Team[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return result.Select(
            x => new F1Team
            {
                Name = x.Name,
                FirstDriver = x.FirstDriver,
                SecondDriver = x.SecondDriver,
            }
        ).ToArray();
    }

    public async Task CreateOrUpdateAsync(F1Team team)
    {
        var existing = (await sqlRepository.FindAsync(x => x.Name == team.Name)).FirstOrDefault();
        if (existing is null)
        {
            await sqlRepository.CreateAsync(
                new F1PredictionTeamStorageElement
                {
                    Id = Guid.NewGuid(),
                    Name = team.Name,
                    FirstDriver = team.FirstDriver,
                    SecondDriver = team.SecondDriver,
                }
            );
            return;
        }

        await sqlRepository.UpdateAsync(
            existing.Id, x =>
            {
                x.FirstDriver = team.FirstDriver;
                x.SecondDriver = team.SecondDriver;
            }
        );
    }

    private readonly ISqlRepository<F1PredictionTeamStorageElement> sqlRepository;
}