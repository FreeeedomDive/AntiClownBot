using AntiClown.Api.Core.Shops.Domain;

namespace AntiClown.Api.Core.Shops.Services;

public interface IShopsValidator
{
    Task ValidateRevealAsync(Shop shop, Guid itemId);
    Task ValidateBuyAsync(Shop shop, Guid itemId);
    Task ValidateReRollAsync(Shop shop);
}