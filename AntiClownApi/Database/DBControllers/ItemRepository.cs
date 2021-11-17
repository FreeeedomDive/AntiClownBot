using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.Models;
using AntiClownBotApi.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace AntiClownBotApi.Database.DBControllers
{
    public class ItemRepository
    {
        private UserRepository UserRepository { get; }
        private DatabaseContext Database { get; }

        public ItemRepository(DatabaseContext database, UserRepository userRepository)
        {
            Database = database;
            UserRepository = userRepository;
        }

        public List<DbItem> GetUserItems(ulong userId) =>
            UserRepository.GetUserWithEconomyAndItems(userId).Items;

        public Enums.SetActiveStatusForItemResult SetActiveStatusForItem(ulong userId, Guid itemId, bool isActive)
        {
            var user = UserRepository.GetUserWithEconomyAndItems(userId);
            var item = user.Items.First(i => i.Id == itemId);
            if (item.ItemType != ItemType.Positive)
            {
                item.IsActive = true;
                Save(item);
                return Enums.SetActiveStatusForItemResult.NegativeItemCantBeInactive;
            }

            if (isActive)
            {
                var activeItemsOfThisType = user.Items.Count(i => i.Name.Equals(item.Name) && i.IsActive);
                if (activeItemsOfThisType >= NumericConstants.MaximumItemsOfOneType)
                {
                    return Enums.SetActiveStatusForItemResult.TooManyActiveItems;
                }
            }

            item.IsActive = isActive;
            Save(item);
            return Enums.SetActiveStatusForItemResult.Success;
        }

        public Enums.SellItemResult SellItem(ulong userId, Guid itemId)
        {
            var user = UserRepository.GetUserWithEconomyAndItems(userId);
            var item = user.Items.First(i => i.Id == itemId);
            var sign = item.ItemType == ItemType.Positive ? 1 : -1;
            var income = sign * item.Price * NumericConstants.SellItemPercent / 100;
            if (income < 0 && user.Economy.ScamCoins < Math.Abs(income))
                return Enums.SellItemResult.NotEnoughMoney;
            user.Economy.ScamCoins += income;
            Database.ItemStats.Remove(item.ItemStats);
            user.Items.Remove(item);
            Database.Items.Remove(item);
            Database.SaveChanges();
            return Enums.SellItemResult.Success;
        }

        public bool TryOpenLootBox(ulong userId, out LootBoxReward reward)
        {
            reward = null;
            var user = UserRepository.GetUserWithEconomyAndItems(userId);
            if (user.Economy.LootBoxes == 0)
            {
                return false;
            }

            reward = new LootBoxReward();

            var lootBoxResult = Randomizer.GetRandomNumberBetween(0, 100);

            if (lootBoxResult < 50)
            {
                var shopItem = DbShopItem.GenerateNewItem();
                reward.Items.Add(ShopRepository.GenerateInventoryItemFromShopItem(shopItem));
            }

            if (lootBoxResult == 0)
            {
                var shopItem = DbShopItem.GenerateNewItem();
                reward.Items.Add(ShopRepository.GenerateInventoryItemFromShopItem(shopItem));
            }

            reward.ScamCoinsReward = Randomizer.GetRandomNumberBetween(100, 500);

            UserRepository.RemoveLootBoxFromUser(user.DiscordId);
            UserRepository.ChangeUserBalance(user.DiscordId, reward.ScamCoinsReward, "Лутбокс");

            foreach (var item in reward.Items)
            {
                UserRepository.AddItemToUserWithOverflow(userId, item.ToDbItem());
            }

            return true;
        }

        private void Save(DbItem item)
        {
            Database.Items.Update(item);
            Database.SaveChanges();
        }
    }
}