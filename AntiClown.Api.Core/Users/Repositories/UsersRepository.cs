using AntiClown.Api.Core.Users.Domain;
using AntiClown.Core.Dto.Exceptions;
using Xdd.HttpHelpers.Models.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
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
        try
        {
            var result = await sqlRepository.ReadAsync(id);
            return mapper.Map<User>(result);
        }
        catch (SqlEntityNotFoundException)
        {
            throw new EntityNotFoundException(id);
        }
    }

    public async Task<User[]> FindAsync(UserFilter filter)
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .WhereIf(filter.DiscordId.HasValue, x => x.DiscordId == filter.DiscordId)
                           .WhereIf(filter.TelegramId.HasValue, x => x.TelegramId == filter.TelegramId)
                           .ToArrayAsync();

        return mapper.Map<User[]>(result);
    }

    public async Task CreateAsync(User user)
    {
        var storageElement = mapper.Map<UserStorageElement>(user);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(User user)
    {
        await sqlRepository.UpdateAsync(
            user.Id, x =>
            {
                x.TelegramId = user.TelegramId;
            }
        );
    }

    private readonly IMapper mapper;

    private readonly ISqlRepository<UserStorageElement> sqlRepository;
}