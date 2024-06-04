/* Generated file */
namespace AntiClown.Api.Client.Users;

public interface IUsersClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Users.UserDto[]> ReadAllAsync();
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Users.UserDto> ReadAsync(System.Guid userId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Users.UserDto[]> FindAsync(AntiClown.Api.Dto.Users.UserFilterDto filter);
    System.Threading.Tasks.Task<System.Guid> CreateAsync(AntiClown.Api.Dto.Users.NewUserDto newUser);
}
