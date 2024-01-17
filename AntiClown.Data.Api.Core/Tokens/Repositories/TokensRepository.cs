using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Data.Api.Core.Tokens.Repositories;

public class TokensRepository : ITokensRepository
{
    public TokensRepository(ISqlRepository<TokenStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<string?> TryReadAsync(Guid userId)
    {
        var result = await sqlRepository.TryReadAsync(userId);
        return result?.Token;
    }

    public async Task CreateOrUpdateAsync(Guid userId, string token)
    {
        var existing = await sqlRepository.TryReadAsync(userId);
        if (existing is not null)
        {
            await sqlRepository.UpdateAsync(
                userId, x =>
                {
                    x.Token = token;
                }
            );
            return;
        }

        await sqlRepository.CreateAsync(
            new TokenStorageElement
            {
                Id = userId,
                Token = token,
            }
        );
    }

    public async Task DeleteAsync(Guid userId)
    {
        await sqlRepository.DeleteAsync(userId);
    }

    private readonly ISqlRepository<TokenStorageElement> sqlRepository;
}