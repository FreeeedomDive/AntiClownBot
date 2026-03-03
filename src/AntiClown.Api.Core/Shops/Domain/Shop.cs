namespace AntiClown.Api.Core.Shops.Domain;

public class Shop
{
    // equal to UserId
    public Guid Id { get; set; }
    public int ReRollPrice { get; set; }
    public int FreeReveals { get; set; }
}