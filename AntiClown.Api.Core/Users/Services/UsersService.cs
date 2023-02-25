using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;

namespace AntiClown.Api.Core.Users.Services;

public class UsersService : IUsersService
{
    public UsersService(IUsersRepository usersRepository)
    {
        this.usersRepository = usersRepository;
    }
    public async Task<User[]> ReadAllAsync()
    {
        return await usersRepository.ReadAllAsync();
    }

    public async Task<User> ReadAsync(Guid id)
    {
        return await usersRepository.ReadAsync(id);
    }

    public async Task<User[]> FindAsync(UserFilter filter)
    {
        return await usersRepository.FindAsync(filter);
    }
    
    private readonly IUsersRepository usersRepository;
}