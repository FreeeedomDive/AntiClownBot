using SqlRepositoryBase.Core.Models;

namespace AntiClown.Data.Api.Core.Tokens.Repositories;

public class TokenStorageElement : SqlStorageElement
{
    // Id == UserId
    public string Token { get; set; }
}