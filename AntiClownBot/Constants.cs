namespace AntiClownBot
{
    public static class Constants
    {
        // стандартный кулдаун подношения
        public const double DefaultCooldown = 60 * 60 * 1000d;
        
        // стандартный начальный социальный рейтинг
        public const int DefaultSocialRating = 500;
        
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