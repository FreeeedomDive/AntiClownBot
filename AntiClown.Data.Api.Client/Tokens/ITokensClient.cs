/* Generated file */
namespace AntiClown.Data.Api.Client.Tokens;

public interface ITokensClient
{
    System.Threading.Tasks.Task InvalidateAsync(System.Guid userId);
    System.Threading.Tasks.Task ValidateAsync(System.Guid userId, AntiClown.Data.Api.Dto.Tokens.TokenDto token);
    System.Threading.Tasks.Task<System.String> GetAsync(System.Guid userId);
}
