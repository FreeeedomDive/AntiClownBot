namespace AntiClown.Data.Api.Core.Tokens.Services;

public interface ITokensService
{
    Task InvalidateAsync(Guid userId);
    Task ValidateAsync(Guid userId, string token);
    Task<string> GetAsync(Guid userId);
}