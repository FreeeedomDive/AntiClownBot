using AntiClown.Api.Core.Users.Domain;

namespace AntiClown.Api.Core.Users.Repositories;

public interface IUsersRepository
{
    Task<User[]> ReadAllAsync();
    Task<User> ReadAsync(Guid id);
    Task<User[]> FindAsync(UserFilter filter);
    Task CreateAsync(User user);
}