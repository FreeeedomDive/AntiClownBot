using AntiClown.Core.Dto.Exceptions;
using AntiClown.Data.Api.Core.Tokens.Repositories;

namespace AntiClown.Data.Api.Core.Tokens.Services;

public class TokensService : ITokensService
{
    public TokensService(
        ITokensRepository tokensRepository,
        ITokenGenerator tokenGenerator
    )
    {
        this.tokensRepository = tokensRepository;
        this.tokenGenerator = tokenGenerator;
    }

    public async Task InvalidateAsync(Guid userId)
    {
        await tokensRepository.DeleteAsync(userId);
    }

    public async Task ValidateAsync(Guid userId, string token)
    {
        Console.WriteLine(token);
        var existingToken = await tokensRepository.TryReadAsync(userId);
        if (existingToken is null || existingToken != token)
        {
            throw new AntiClownUnauthorizedException($"Wrong token was provided for user {userId}");
        }
    }

    public async Task<string> GetAsync(Guid userId)
    {
        var existingToken = await tokensRepository.TryReadAsync(userId);
        if (existingToken is not null)
        {
            return existingToken;
        }

        var newToken = tokenGenerator.Generate();
        await tokensRepository.CreateOrUpdateAsync(userId, newToken);
        return newToken;
    }

    private readonly ITokenGenerator tokenGenerator;
    private readonly ITokensRepository tokensRepository;
}