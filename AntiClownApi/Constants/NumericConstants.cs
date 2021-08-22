namespace AntiClownBotApi.Constants
{
    public class NumericConstants
    {
        
        public const ulong BotId = 760879629509853224;

        public const ulong GuildId = 277096298761551872;

        public const ulong BotChannelId = 838477706643374090;
        
        // максимальное количество предметов одного типа в инвентаре
        public const int MaximumItemsOfOneType = 3;

        // максимальное количество предметов в магазине, доступных для покупки
        public const int MaximumItemsInShop = 5;
        
        // стандартный кулдаун подношения
        public const double DefaultCooldown = 60 * 60 * 1000d;

        // начальная цена реролла магазина
        public const int DefaultReRollPrice = 200;

        // увеличение цены реролла
        public const int DefaultReRollPriceIncrease = 50;
        
        // стандартный начальный социальный рейтинг
        public const int DefaultScamCoins = 1000;
        
        // кулдаун уменьшается на 10% при срабатывании одного гигабайта
        public const double CooldownDecreaseByOneGigabyteItem = 0.1;
        
        // 10% шанс срабатывания гигабайта, чтобы уменьшить кулдаун
        public const int CooldownDecreaseChanceByOneGigabyte = 5;
        
        // 2% шанс срабатывания нефритового стержня для увеличения кулдауна
        public const int CooldownIncreaseChanceByOneJade = 2;
        
        // увеличение кулдауна в 2 раза при срабатывании стержня
        public const int CooldownIncreaseByOneJade = 2;
        
        // стартовое значение логарифмического распределения для кошки-жены и автоматического подношения
        public const int LogarithmicDistributionStartValueForCatWife = 16;
        
        // стартовое значение возможного получения рейтинга с подношения
        public const int MinTributeValue = -40;
        
        // конечное значение возможного получения рейтинга с подношения
        public const int MaxTributeValue = 100;
        
        // уменьшение правой границы возможного получения рейтинга с подношения за 1 миску риса
        public const int TributeDecreaseByOneRiceBowl = 2;
        
        // увеличение правой границы возможного получения рейтинга с подношения за 1 миску риса
        public const int TributeIncreaseByOneRiceBowl = 5;
        
        // стартовое значение логарифмического распределения для собаки-жены и уклонения от пидора
        public const int LogarithmicDistributionStartValueForDogWife = 16;
        
        // стартовое значение логарифмического распределения для срабатывания коммунизма
        public const int LogarithmicDistributionStartValueForCommunism = 4;
    }
}