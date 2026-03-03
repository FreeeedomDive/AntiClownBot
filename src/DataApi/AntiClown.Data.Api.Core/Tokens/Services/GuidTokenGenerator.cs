namespace AntiClown.Data.Api.Core.Tokens.Services;

public class GuidTokenGenerator : ITokenGenerator
{
    public string Generate()
    {
        return Guid.NewGuid().ToString();
    }
}