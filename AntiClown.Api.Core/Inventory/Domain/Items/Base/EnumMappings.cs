namespace AntiClown.Api.Core.Inventory.Domain.Items.Base;

public static class EnumMappings
{
    public static readonly Dictionary<ItemType, ItemName[]> TypeToNames = new()
    {
        { ItemType.Positive, new[] { ItemName.CatWife, ItemName.DogWife, ItemName.Internet, ItemName.RiceBowl } },
        { ItemType.Negative, new[] { ItemName.CommunismBanner, ItemName.JadeRod } },
    };

    public static readonly Dictionary<ItemType, bool> TypeToDefaultActiveStatus = new()
    {
        { ItemType.Positive, false },
        { ItemType.Negative, true },
    };

    public static readonly Dictionary<ItemName, ItemType> NameToType = new()
    {
        { ItemName.CatWife, ItemType.Positive },
        { ItemName.DogWife, ItemType.Positive },
        { ItemName.Internet, ItemType.Positive },
        { ItemName.RiceBowl, ItemType.Positive },
        { ItemName.CommunismBanner, ItemType.Negative },
        { ItemName.JadeRod, ItemType.Negative },
    };

    public static readonly Dictionary<ItemType, int> TypeToRandomnessWeight = new()
    {
        { ItemType.Positive, 5 },
        { ItemType.Negative, 1 },
    };
}