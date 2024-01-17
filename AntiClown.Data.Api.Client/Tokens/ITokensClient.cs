namespace AntiClown.Data.Api.Client.Tokens;

public interface ITokensClient
{
    Task InvalidateAsync(Guid userId);
    Task ValidateAsync(Guid userId, string token);
    Task<string> GetAsync(Guid userId);
}