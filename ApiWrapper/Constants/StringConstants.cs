using System.Collections.Generic;
using System.Linq;

namespace ApiWrapper.Constants
{
    public static class StringConstants
    {
        public const string CatWifeName = "Кошка-жена";
        public const string DogWifeName = "Собака-жена";
        public const string InternetName = "Интернет";
        public const string RiceBowlName = "Рис миска";
        public const string JadeRodName = "Нефритовый стержень";
        public const string CommunismBannerName = "Коммунистический плакат";

        public static readonly List<string> GoodItemNames = new()
        {
            CatWifeName,
            DogWifeName,
            InternetName,
            RiceBowlName
        };

        public static readonly List<string> BadItemNames = new()
        {
            JadeRodName,
            CommunismBannerName
        };

        public static readonly List<string> AllItemsNames = GoodItemNames.Union(BadItemNames).ToList();
    }
}