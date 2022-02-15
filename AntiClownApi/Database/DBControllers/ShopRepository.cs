using System;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.Models.ItemBuilders;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Database.DBControllers
{
    public class ShopRepository
    {
        private UserRepository UserRepository { get; }
        private DatabaseContext Database { get; }

        public ShopRepository(DatabaseContext database, UserRepository userRepository)
        {
            Database = database;
            UserRepository = userRepository;
        }

        public static BaseItem GenerateInventoryItemFromShopItem(DbShopItem shopItem)
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
                    .WithRandomDistributedStats()
                    .Build(),
                StringConstants.RiceBowlName => item
                    .AsRiceBowl()
                    .WithRandomTributeIncrease()
                    .WithRandomTributeDecrease()
                    .WithRandomDistributedStats()
                    .Build(),
                StringConstants.CommunismBannerName => item
                    .AsCommunismBanner()
                    .WithRandomDistributedStats()
                    .Build(),
                StringConstants.JadeRodName => item
                    .AsJadeRod()
                    .WithRandomDistributedStats()
                    .Build(),
                _ => throw new ArgumentOutOfRangeException($"Invalid item name {shopItem.Name}")
            };
        }

        public void Save()
        {
            Database.SaveChanges();
        }

        public Guid GetShopItemInSlot(ulong userId, int slot)
        {
            var user = UserRepository.GetUserWithShop(userId);
            return user.Shop.Items[slot - 1].Id;
        }

        public Enums.RevealResult RevealItem(ulong userId, Guid itemId, out DbShopItem revealedItem)
        {
            revealedItem = null;
            var user = UserRepository.GetUserWithShop(userId);
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
                UserRepository.ChangeUserBalance(userId, -revealCost,
                    $"Покупка распознавания предмета {shopItem.Name}");
            }

            shopItem.IsRevealed = true;
            UserRepository.Save();
            Save();
            revealedItem = shopItem;
            return Enums.RevealResult.Success;
        }

        public Enums.BuyResult TryBuyItem(ulong userId, Guid itemId, out BaseItem newItem)
        {
            newItem = null;
            var user = UserRepository.GetUserWithShop(userId);
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

            dbItem.IsActive = dbItem.ItemType == ItemType.Negative;

            UserRepository.AddItemToUserWithOverflow(userId, dbItem);

            UserRepository.ChangeUserBalance(userId, -newItem.Price, $"Покупка предмета {dbItem.Name}");
            shopItem.IsOwned = true;

            Save();

            return Enums.BuyResult.Success;
        }

        public DbUser GetUserShop(ulong userId)
        {
            return UserRepository.GetUserWithShop(userId);
        }

        public Enums.ReRollResult ReRollShop(ulong userId)
        {
            var user = UserRepository.GetUserWithShop(userId);

            if (user.Shop.ReRollPrice > 0 && user.Economy.ScamCoins < user.Shop.ReRollPrice)
                return Enums.ReRollResult.NotEnoughMoney;

            user.Shop = DbUserShop.GenerateNewItemsForShop(user.Shop);
            UserRepository.ChangeUserBalance(userId, -user.Shop.ReRollPrice, "Реролл магазина");
            user.Shop.ReRollPrice = Math.Max(NumericConstants.DefaultReRollPrice,
                user.Shop.ReRollPrice + NumericConstants.DefaultReRollPriceIncrease);

            UserRepository.Save();
            Save();

            return Enums.ReRollResult.Success;
        }

        public void ResetReRollPriceForAllUsers()
        {
            Database.Users.ForEach(user => user.Shop.ReRollPrice = 0);
            UserRepository.Save();
            Save();
        }

        public void ResetFreeReveals(ulong userId)
        {
            var user = UserRepository.GetUserWithShop(userId);

            user.Shop.FreeItemReveals = Math.Max(NumericConstants.FreeItemRevealsPerDay, user.Shop.FreeItemReveals);
            UserRepository.Save();
        }

        public void AddFreeReveals(ulong userId, int count)
        {
            var user = UserRepository.GetUserWithShop(userId);

            user.Shop.FreeItemReveals += count;
            UserRepository.Save();
            Save();
        }

        public void ResetFreeRevealsForAllUsers()
        {
            var users = UserRepository.GetAllUsersIds();
            users.ForEach(ResetFreeReveals);
        }
    }
}