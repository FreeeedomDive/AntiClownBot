namespace AntiClown.Api.Core.Inventory.Domain;

// TODO: вынести в отдельный репозиторий? есть потенциал потом это вынести на редактирование в админку
public static class Constants
{
    // максимальное количество активных предметов одного типа в инвентаре
    public const int MaximumActiveItemsOfOneType = 3;

    // максимальное количество предметов в магазине, доступных для покупки
    public const int MaximumItemsInShop = 5;

    // стандартный кулдаун подношения
    public const double DefaultCooldown = 60 * 60 * 1000d;

    // начальная цена реролла магазина
    public const int DefaultReRollPrice = 100;

    // увеличение цены реролла
    public const int DefaultReRollPriceIncrease = 25;

    // стандартный начальный социальный рейтинг
    public const int DefaultScamCoins = 1000;

    // 2% шанс срабатывания нефритового стержня для увеличения кулдауна
    public const int CooldownIncreaseChanceByOneJade = 2;

    // стартовое значение возможного получения рейтинга с подношения
    public const int MinTributeValue = -40;

    // конечное значение возможного получения рейтинга с подношения
    public const int MaxTributeValue = 100;

    // количество бесплатных ежедневно выдающихся распознавателей предметов
    public const int FreeItemRevealsPerDay = 2;

    // процент от стоимости предмета, который получает пользователь за продажу
    public const int SellItemPercent = 50;
}