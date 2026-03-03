/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Data.Api.Client.Tokens;

public interface ITokensClient
{
    Task InvalidateAsync(System.Guid userId);
    Task ValidateAsync(System.Guid userId, AntiClown.Data.Api.Dto.Tokens.TokenDto token);
    Task<string> GetAsync(System.Guid userId);
}
