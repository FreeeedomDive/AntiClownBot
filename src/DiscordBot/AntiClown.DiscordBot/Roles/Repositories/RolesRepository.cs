using SqlRepositoryBase.Core.Repository;

namespace AntiClown.DiscordBot.Roles.Repositories;

public class RolesRepository : IRolesRepository
{
    public RolesRepository(ISqlRepository<RoleStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<ulong[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return result.Select(x => x.DiscordRoleId).ToArray();
    }

    public async Task<bool> ExistsAsync(ulong roleId)
    {
        return (await sqlRepository.FindAsync(x => x.DiscordRoleId == roleId)).Length > 0;
    }

    public async Task CreateAsync(ulong discordRoleId)
    {
        await sqlRepository.CreateAsync(
            new RoleStorageElement
            {
                Id = Guid.NewGuid(),
                DiscordRoleId = discordRoleId,
            }
        );
    }

    private readonly ISqlRepository<RoleStorageElement> sqlRepository;
}