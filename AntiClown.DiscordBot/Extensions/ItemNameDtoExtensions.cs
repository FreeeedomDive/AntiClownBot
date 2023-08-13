using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions;

public static class ItemNameDtoExtensions
{
    public static string Localize(this ItemNameDto itemName)
    {
        return itemName switch
        {
            ItemNameDto.CatWife => "Кошка-жена",
            ItemNameDto.CommunismBanner => "Коммунистический плакат",
            ItemNameDto.DogWife => "Собака-жена",
            ItemNameDto.Internet => "Интернет",
            ItemNameDto.JadeRod => "Нефритовый стержень",
            ItemNameDto.RiceBowl => "Рис миска",
            _ => throw new ArgumentOutOfRangeException(nameof(itemName), itemName, null),
        };
    }
}