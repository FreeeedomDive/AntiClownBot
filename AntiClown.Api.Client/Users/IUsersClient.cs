using AntiClown.Api.Dto.Users;

namespace AntiClown.Api.Client.Users;

public interface IUsersClient
{
    Task<UserDto[]> ReadAllAsync();
    Task<UserDto> ReadAsync(Guid userId);
    Task<UserDto[]> FindAsync(UserFilterDto filter);
    Task<Guid> CreateAsync(NewUserDto newUserDto);
}