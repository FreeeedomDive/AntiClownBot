using AntiClown.DiscordBot.Releases.Domain;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class ReleasesRepository(ISqlRepository<ReleaseVersionStorageElement> sqlRepository) : IReleasesRepository
{
    public async Task CreateAsync(ReleaseVersion releaseVersion)
    {
        var storageElement = new ReleaseVersionStorageElement
        {
            Id = Guid.NewGuid(),
            Major = releaseVersion.Major,
            Minor = releaseVersion.Minor,
            Patch = releaseVersion.Patch,
            Description = releaseVersion.Description,
            CreatedAt = releaseVersion.CreatedAt,
        };
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task<ReleaseVersion?> ReadLastAsync()
    {
        var queryable = await sqlRepository.BuildCustomQueryAsync();
        var result = await queryable
                           .OrderByDescending(x => x.CreatedAt)
                           .FirstOrDefaultAsync();
        return result is null
            ? null
            : new ReleaseVersion
            {
                Major = result.Major,
                Minor = result.Minor,
                Patch = result.Patch,
                Description = result.Description,
                CreatedAt = result.CreatedAt,
            };
    }
}