using System;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.ItemBuilders;
using AntiClownBotApi.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace AntiClownBotApi.Database.DBControllers
{
    public static class ShopDbController
    {
        private static DbUser GetUserWithShopUsingConnection(ulong userId, DatabaseContext database)
        {
            return UserDbController.IsUserExist(userId)
                ? database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Shop)
                    .Include(u => u.Items)
                    .Include(u => u.Shop.Items)
                    .First(u => u.DiscordId == userId)
                : UserDbController.CreateNewUserWithDbConnection(userId, database);
        }

        private static BaseItem GenerateInventoryItemFromShopItem(DbShopItem shopItem)
        {
            var item = new ItemBuilder()
                .WithRarity(shopItem.Rarity)
                .WithPrice(shopItem.Price);
            return shopItem.Name switch
            {
                StringConstants.CatWifeName => item
                    .AsCatWife()
                    .WithRandomAutoTributeChance()
                    .Build(),
                StringConstants.DogWifeName => item
                    .AsDogWife()
                    .WithRandomLootBoxFindChance()
                    .Build(),
                StringConstants.InternetName => item
                    .AsInternet()
                    .WithRandomCooldownReduceChance()
                    .WithRandomCooldownReducePercent()
                    .WithRandomCooldownReduceTries()
                    .Build(),
                StringConstants.RiceBowlName => item
                    .AsRiceBowl()
                    .WithRandomTributeIncrease()
                    .WithRandomTributeDecrease()
                    .WithRandomDistributedStats()
                    .Build(),
                // StringConstants.CommunismBannerName => item
                //     .As()
                //     .WithRandomAutoTributeChance()
                //     .Build(),
                // StringConstants.JadeRodName => item.AsCatWife().WithRandomAutoTributeChance().Build(),
                _ => throw new ArgumentOutOfRangeException($"Invalid item name {shopItem.Name}")
            };
        }

        public static Guid GetShopItemInSlot(ulong userId, int slot)
        {
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);
            return user.Shop.Items[slot - 1].Id;
        }

        public static Enums.RevealResult RevealItem(ulong userId, Guid itemId, out DbShopItem revealedItem)
        {
            revealedItem = null;
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);
            if (user.Shop.Items.All(i => i.Id != itemId))
                return Enums.RevealResult.ItemDoesntExistInShop;
            var shopItem = user.Shop.Items.First(i => i.Id == itemId);
            if (shopItem.IsOwned)
                return Enums.RevealResult.AlreadyBought;
            if (shopItem.IsRevealed)
                return Enums.RevealResult.AlreadyRevealed;

            if (user.Shop.FreeItemReveals > 0)
            {
                user.Shop.FreeItemReveals--;
            }
            else
            {
                var revealCost = shopItem.Price * 4 / 10;
                if (user.Economy.ScamCoins < revealCost)
                    return Enums.RevealResult.NotEnoughMoney;
                UserDbController.ChangeUserBalanceWithConnection(userId, -revealCost,
                    $"Покупка распознавания предмета {shopItem.Name}", database);
            }

            shopItem.IsRevealed = true;
            database.SaveChanges();
            revealedItem = shopItem;
            return Enums.RevealResult.Success;
        }

        public static Enums.BuyResult TryBuyItem(ulong userId, Guid itemId, out BaseItem newItem)
        {
            newItem = null;
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);
            if (user.Shop.Items.All(i => i.Id != itemId))
                return Enums.BuyResult.ItemDoesntExistInShop;
            var shopItem = user.Shop.Items.First(i => i.Id == itemId);
            if (shopItem.IsOwned)
                return Enums.BuyResult.AlreadyBought;
            if (user.Economy.ScamCoins < shopItem.Price)
                return Enums.BuyResult.NotEnoughMoney;
            // // // // if (user.Items.Count(i => i.Name == shopItem.Name) > NumericConstants.MaximumItemsOfOneType)
            // // // //     return Enums.BuyResult.TooManyItemsOfSelectedType;

            newItem = GenerateInventoryItemFromShopItem(shopItem);
            var dbItem = newItem.ToDbItem();
            
            AddItemWithOverflow(user, dbItem, database);
            
            UserDbController.ChangeUserBalanceWithConnection(userId, -newItem.Price,
                $"Покупка предмета {dbItem.Name}", database);
            shopItem.IsOwned = true;

            database.SaveChanges();

            return Enums.BuyResult.Success;
        }

        private static void AddItemWithOverflow(DbUser user, DbItem dbItem, DatabaseContext database)
        {
            var itemsOfType = user.Items.Where(i => i.Name == dbItem.Name).ToList();
            if (itemsOfType.Count == NumericConstants.MaximumItemsOfOneType)
            {
                var itemToDelete = itemsOfType.OrderBy(i => i.Rarity).First();
                database.ItemStats.Remove(itemToDelete.ItemStats);
                user.Items.Remove(itemToDelete);
                database.Items.Remove(itemToDelete);
            }
            dbItem.User = user;
            database.Items.Add(dbItem);
            user.Items.Add(dbItem);
        }

        public static DbUser GetUserShop(ulong userId)
        {
            using var database = new DatabaseContext();
            return GetUserWithShopUsingConnection(userId, database);
        }

        public static Enums.ReRollResult ReRollShop(ulong userId)
        {
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);

            if (user.Economy.ScamCoins < user.Shop.ReRollPrice)
                return Enums.ReRollResult.NotEnoughMoney;

            user.Shop = DbUserShop.GenerateNewShopForUser(userId);
            UserDbController.ChangeUserBalanceWithConnection(userId, -user.Shop.ReRollPrice, "Реролл магазина", database);
            user.Shop.ReRollPrice += NumericConstants.DefaultReRollPriceIncrease;
            database.SaveChanges();

            return Enums.ReRollResult.Success;
        }

        public static void ResetReRollPrice(ulong userId)
        {
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);

            user.Shop.ReRollPrice = NumericConstants.DefaultReRollPrice;
            database.SaveChanges();
        }

        public static void ResetReRollPriceForAllUsers()
        {
            using var database = new DatabaseContext();
            database.Users.ForEach(user => ResetReRollPrice(user.DiscordId));
            database.SaveChanges();
        }

        public static void ResetFreeReveals(ulong userId)
        {
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);

            user.Shop.FreeItemReveals = Math.Max(1, user.Shop.FreeItemReveals);
            database.SaveChanges();
        }

        public static void AddFreeReveals(ulong userId, int count)
        {
            using var database = new DatabaseContext();
            var user = GetUserWithShopUsingConnection(userId, database);

            user.Shop.FreeItemReveals += count;
            database.SaveChanges();
        }

        public static void ResetFreeRevealsForAllUsers()
        {
            using var database = new DatabaseContext();
            database.Users.ForEach(user => ResetFreeReveals(user.DiscordId));
            database.SaveChanges();
        }
    }
}