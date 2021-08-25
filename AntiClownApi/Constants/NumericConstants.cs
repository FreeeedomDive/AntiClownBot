namespace AntiClownBotApi.Constants
{
    public static class NumericConstants
    {
        // максимальное количество предметов одного типа в инвентаре
        public const int MaximumItemsOfOneType = 3;

        // максимальное количество предметов в магазине, доступных для покупки
        public const int MaximumItemsInShop = 5;
        
        // стандартный кулдаун подношения
        public const double DefaultCooldown = 60 * 60 * 1000d;

        // начальная цена реролла магазина
        public const int DefaultReRollPrice = 200;

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
    }
}