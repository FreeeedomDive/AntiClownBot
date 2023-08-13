using AntiClown.Api.Core.Users.Domain;

namespace AntiClown.Api.Core.Users.Services;

public interface INewUserService
{
    Task<Guid> CreateNewUserAsync(NewUser newUser);
}