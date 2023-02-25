using AntiClown.Api.Core.Users.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Users.Repositories;

public class UsersRepository : IUsersRepository
{
    public UsersRepository(
        ISqlRepository<UserStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<User[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return mapper.Map<User[]>(result);
    }

    public async Task<User> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return mapper.Map<User>(result);
    }

    public async Task<User[]> FindAsync(UserFilter filter)
    {
        var result = await sqlRepository
            .BuildCustomQuery()
            .WhereIf(filter.DiscordId.HasValue, x => x.DiscordId == filter.DiscordId)
            .ToArrayAsync();

        return mapper.Map<User[]>(result);
    }

    public async Task CreateAsync(User user)
    {
        var storageElement = mapper.Map<UserStorageElement>(user);
        await sqlRepository.CreateAsync(storageElement);
    }

    private readonly ISqlRepository<UserStorageElement> sqlRepository;
    private readonly IMapper mapper;
}