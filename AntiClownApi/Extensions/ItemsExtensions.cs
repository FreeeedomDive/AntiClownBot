using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Extensions
{
    public static class ItemsExtensions
    {
        public static IEnumerable<RiceBowl> RiceBowls(this IEnumerable<DbItem> items) => items
            .Where(item => item.Name.Equals(StringConstants.RiceBowlName))
            .Select(item => (RiceBowl) item);
        
        public static IEnumerable<CommunismBanner> CommunismBanners(this IEnumerable<DbItem> items) => items
            .Where(item => item.Name.Equals(StringConstants.CommunismBannerName))
            .Select(item => (CommunismBanner) item);

        public static IEnumerable<CatWife> CatWives(this IEnumerable<DbItem> items) => items
            .Where(item => item.Name.Equals(StringConstants.CatWifeName))
            .Select(item => (CatWife) item);

        public static IEnumerable<DogWife> DogWives(this IEnumerable<DbItem> items) => items
            .Where(item => item.Name.Equals(StringConstants.DogWifeName))
            .Select(item => (DogWife) item);

        public static IEnumerable<JadeRod> JadeRods(this IEnumerable<DbItem> items) => items
            .Where(item => item.Name.Equals(StringConstants.JadeRodName))
            .Select(item => (JadeRod) item);

        public static IEnumerable<Internet> Internets(this IEnumerable<DbItem> items) => items
            .Where(item => item.Name.Equals(StringConstants.InternetName))
            .Select(item => (Internet) item);
    }
}