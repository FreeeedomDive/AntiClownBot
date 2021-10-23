using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Models.Items
{
    public abstract class BaseItem
    {
        public BaseItem(Guid id)
        {
            Id = id;
        }
        
        public Guid Id { get; }
        public abstract string Name { get; }
        public abstract ItemType ItemType { get; } 
        public Rarity Rarity { get; init; }
        public int Price { get; set; }
        public bool IsActive { get; set; }

        public abstract DbItem ToDbItem();

        public static BaseItem FromDbItem(DbItem item) => item.Name switch
        {
            StringConstants.CatWifeName => (CatWife) item,
            StringConstants.DogWifeName => (DogWife) item,
            StringConstants.RiceBowlName => (RiceBowl) item,
            StringConstants.InternetName => (Internet) item,
            StringConstants.JadeRodName => (JadeRod) item,
            StringConstants.CommunismBannerName => (CommunismBanner) item,
            _ => throw new ArgumentOutOfRangeException(nameof(item))
        };
    }
}