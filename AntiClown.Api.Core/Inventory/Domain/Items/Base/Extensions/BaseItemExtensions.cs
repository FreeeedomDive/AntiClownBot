namespace AntiClown.Api.Core.Inventory.Domain.Items.Base.Extensions;

public static class BaseItemExtensions
{
    public static IEnumerable<CatWife> CatWives(this IEnumerable<BaseItem> items)
    {
        return items
            .Where(item => item.ItemName == ItemName.CatWife)
            .Select(item => (item as CatWife)!);
    }
    
    public static IEnumerable<CommunismBanner> CommunismBanners(this IEnumerable<BaseItem> items)
    {
        return items
            .Where(item => item.ItemName == ItemName.CommunismBanner)
            .Select(item => (item as CommunismBanner)!);
    }
    
    public static IEnumerable<DogWife> DogWives(this IEnumerable<BaseItem> items)
    {
        return items
            .Where(item => item.ItemName == ItemName.DogWife)
            .Select(item => (item as DogWife)!);
    }
    
    public static IEnumerable<Internet> Internets(this IEnumerable<BaseItem> items)
    {
        return items
            .Where(item => item.ItemName == ItemName.Internet)
            .Select(item => (item as Internet)!);
    }
    
    public static IEnumerable<JadeRod> JadeRods(this IEnumerable<BaseItem> items)
    {
        return items
            .Where(item => item.ItemName == ItemName.JadeRod)
            .Select(item => (item as JadeRod)!);
    }
    
    public static IEnumerable<RiceBowl> RiceBowls(this IEnumerable<BaseItem> items)
    {
        return items
            .Where(item => item.ItemName == ItemName.RiceBowl)
            .Select(item => (item as RiceBowl)!);
    }
}