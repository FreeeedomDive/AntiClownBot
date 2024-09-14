/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Users;

public interface IUsersClient
{
    Task<AntiClown.Api.Dto.Users.UserDto[]> ReadAllAsync();
    Task<AntiClown.Api.Dto.Users.UserDto> ReadAsync(System.Guid userId);
    Task<AntiClown.Api.Dto.Users.UserDto[]> FindAsync(AntiClown.Api.Dto.Users.UserFilterDto filter);
    Task<System.Guid> CreateAsync(AntiClown.Api.Dto.Users.NewUserDto newUser);
    Task BindTelegramAsync(System.Guid userId, long telegramId);
}
