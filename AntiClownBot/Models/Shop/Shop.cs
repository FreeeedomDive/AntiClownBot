using AntiClownBot.Models.User.Inventory;
using AntiClownBot.Models.User.Inventory.Items;
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
        public ulong ShopBuyMessageId;
        private Dictionary<Item, List<ulong>> itemsToBuy = new Dictionary<Item, List<ulong>>
        {
            {new CatWife(), new List<ulong>() },
            {new DogWife(), new List<ulong>() },
            {new Gigabyte(), new List<ulong>() },
            {new RiceBowl(), new List<ulong>() }
        };
        public ulong ShopSellMessageId;
        private Dictionary<Item, List<ulong>> itemsToSell = new Dictionary<Item, List<ulong>>
        {
            {new CatWife(), new List<ulong>() },
            {new DogWife(), new List<ulong>() },
            {new Gigabyte(), new List<ulong>() },
            {new RiceBowl(), new List<ulong>() },
            {new JadeRod() , new List<ulong>() },
            {new CommunismPoster(), new List<ulong>() }
        };
        public TransactionResult BuyItem(Item item, SocialRatingUser user)
        {
            if (itemsToBuy[item].Contains(user.DiscordId))
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.LimitReached,
                    Result = null
                };
            }
            if (user.SocialRating < item.Price)
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.NotEnoughMoney,
                    Result = null
                };
            }
            itemsToBuy[item].Add(user.DiscordId);
            user.AddCustomItem(item);
            user.ChangeRating(-item.Price);
            return new TransactionResult
            {
                Status = TransactionStatus.Success,
                Result = $"{user.DiscordUsername} купил(а) {item.Name}"
            };
        }
        public TransactionResult SellItem(Item item, SocialRatingUser user)
        {
            if (itemsToSell[item].Contains(user.DiscordId))
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.LimitReached,
                    Result = null
                };
            }
            if(user.Items[item] < 1)
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.NotEnoughItems,
                    Result = null
                };
            }
            if (user.SocialRating < -item.Price/2)
            {
                return new TransactionResult
                {
                    Status = TransactionStatus.NotEnoughMoney,
                    Result = null
                };
            }
            itemsToSell[item].Add(user.DiscordId);
            user.ChangeRating(item.Price/2);
            user.RemoveCustomItem(item);
            return new TransactionResult
            {
                Status = TransactionStatus.Success,
                Result = $"{user.DiscordUsername} продал(а) {item.Name}"
            };
        }
    }
}
