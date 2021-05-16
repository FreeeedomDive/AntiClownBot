using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Shop
{
    public class Shop
    {
        public enum TransactionStatus
        {
            Success,
            NotEnoughMoney,
            LimitReached,
            NotEnoughItems
        }
        public class TransactionResult
        {
            public TransactionStatus Status;
            public string Result;
        }
        public class Product
        {
            public InventoryItem Item;
            public int Price;
        }
        public static Dictionary<InventoryItem, int> BuyPrices = new Dictionary<InventoryItem, int>
        {  
            {InventoryItem.CatWife , 1000},
            {InventoryItem.DogWife, 1000 },
            {InventoryItem.Gigabyte, 1000 },
            {InventoryItem.RiceBowl, 1000 }
        };
        public static Dictionary<InventoryItem, int> SellPrices = new Dictionary<InventoryItem, int>
        {
            {InventoryItem.CatWife , 500},
            {InventoryItem.DogWife, 500 },
            {InventoryItem.Gigabyte, 500 },
            {InventoryItem.RiceBowl, 500 },
            {InventoryItem.CommunismPoster, 2000 },
            {InventoryItem.JadeRod, 2000 }
        };
        public ulong ShopBuyMessageId;
        private Dictionary<InventoryItem, List<ulong>> itemsToBuy = new Dictionary<InventoryItem, List<ulong>>
        {
            {InventoryItem.CatWife, new List<ulong>() },
            {InventoryItem.DogWife, new List<ulong>() },
            {InventoryItem.Gigabyte, new List<ulong>() },
            {InventoryItem.RiceBowl, new List<ulong>() }
        };
        public ulong ShopSellMessageId;
        private Dictionary<InventoryItem, List<ulong>> itemsToSell = new Dictionary<InventoryItem, List<ulong>>
        {
            {InventoryItem.CatWife, new List<ulong>() },
            {InventoryItem.DogWife, new List<ulong>() },
            {InventoryItem.Gigabyte, new List<ulong>() },
            {InventoryItem.RiceBowl, new List<ulong>() },
            {InventoryItem.JadeRod , new List<ulong>() },
            {InventoryItem.CommunismPoster, new List<ulong>() }
        };
        public TransactionResult BuyItem(InventoryItem item, SocialRatingUser user)
        {
            if (itemsToBuy[item].Contains(user.DiscordId))
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.LimitReached,
                    Result = null
                };
            }
            if (user.SocialRating < BuyPrices[item])
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.NotEnoughMoney,
                    Result = null
                };
            }
            itemsToBuy[item].Add(user.DiscordId);
            user.AddCustomItem(item);
            user.ChangeRating(-BuyPrices[item]);
            return new TransactionResult
            {
                Status = TransactionStatus.Success,
                Result = $"{user.DiscordUsername} купил(а) {Utility.ItemToString(item)}"
            };
        }
        public TransactionResult SellItem(InventoryItem item, SocialRatingUser user)
        {
            if (itemsToSell[item].Contains(user.DiscordId))
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.LimitReached,
                    Result = null
                };
            }
            if(user.UserItems[item] < 1)
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.NotEnoughItems,
                    Result = null
                };
            }
            if(item == InventoryItem.CommunismPoster || item == InventoryItem.JadeRod)
            {
                if(user.SocialRating < SellPrices[item])
                {
                    return new TransactionResult
                    {
                        Status = TransactionStatus.NotEnoughMoney,
                        Result = null
                    };
                }
                itemsToSell[item].Add(user.DiscordId);
                user.ChangeRating(-SellPrices[item]);
                user.RemoveCustomItem(item);
                return new TransactionResult
                {
                    Status = TransactionStatus.Success,
                    Result = $"{user.DiscordUsername} избавился(ась) от {Utility.ItemToString(item)}"
                };
            }
            itemsToSell[item].Add(user.DiscordId);
            user.ChangeRating(SellPrices[item]);
            user.RemoveCustomItem(item);
            return new TransactionResult
            {
                Status = TransactionStatus.Success,
                Result = $"{user.DiscordUsername} продал(а) {Utility.ItemToString(item)}"
            };
        }
    }
}
