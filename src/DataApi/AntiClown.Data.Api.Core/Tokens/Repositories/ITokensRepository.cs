namespace AntiClown.Data.Api.Core.Tokens.Repositories;

public interface ITokensRepository
{
    Task<string?> TryReadAsync(Guid userId);
    Task CreateOrUpdateAsync(Guid userId, string token);
    Task DeleteAsync(Guid userId);
}