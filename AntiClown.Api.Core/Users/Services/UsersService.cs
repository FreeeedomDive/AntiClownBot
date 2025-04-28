using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;

namespace AntiClown.Api.Core.Users.Services;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
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

    public async Task BindTelegramUserIdAsync(Guid id, long telegramUserId)
    {
        var user = await usersRepository.ReadAsync(id);
        user.TelegramId = telegramUserId;
        await usersRepository.UpdateAsync(user);
    }

    public async Task BindYandexIdAsync(Guid id, string? yandexUserId)
    {
        var user = await usersRepository.ReadAsync(id);
        user.YandexId = yandexUserId;
        await usersRepository.UpdateAsync(user);
    }
}