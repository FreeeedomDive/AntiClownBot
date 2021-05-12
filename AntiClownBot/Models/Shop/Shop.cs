using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Shop
{
    public class Shop
    {
        public enum BuyStatus
        {
            Success,
            NotEnoughMoney,
            LimitReached
        }
        public class BuyResult
        {
            public BuyStatus Status;
            public string Result;
        }
        public ulong ShopMessageId;
        private Dictionary<InventoryItem, List<ulong>> itemsToBuy = new Dictionary<InventoryItem, List<ulong>>
        {
            {InventoryItem.CatWife, new List<ulong>() },
            {InventoryItem.DogWife, new List<ulong>() },
            {InventoryItem.Gigabyte, new List<ulong>() },
            {InventoryItem.RiceBowl, new List<ulong>() }
        };

        public BuyResult BuyItem(InventoryItem item, SocialRatingUser user)
        {
            if(itemsToBuy[item].Contains(user.DiscordId))
            {
                return new BuyResult
                {
                    Status = BuyStatus.LimitReached,
                    Result = null
                };
            }
            if(user.SocialRating < 1000)
            {
                return new BuyResult
                {
                    Status = BuyStatus.NotEnoughMoney,
                    Result = null
                };
            }
            itemsToBuy[item].Add(user.DiscordId);
            user.AddCustomItem(item);
            user.ChangeRating(-1000);
            return new BuyResult
            {
                Status = BuyStatus.Success,
                Result = $"{user.DiscordUsername} купил {Utility.ItemToString(item)}"
            };
        }
    }
}
