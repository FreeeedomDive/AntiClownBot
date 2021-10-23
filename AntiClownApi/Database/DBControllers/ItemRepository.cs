using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.DTO.Responses;
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
                return Enums.SetActiveStatusForItemResult.NegativeItemCantBeInactive;
            }
            item.IsActive = isActive;
            var entry = Database.Entry(item);
            entry.State = EntityState.Modified;
            Database.Items.Update(item);
            Database.SaveChanges();
            return Enums.SetActiveStatusForItemResult.Success;
        }

        public Enums.SellItemResult SellItem(ulong userId, Guid itemId)
        {
            var user = UserRepository.GetUserWithEconomyAndItems(userId);
            var item = user.Items.First(i => i.Id == itemId);
            var sign = item.ItemType == ItemType.Positive ? 1 : -1;
            var income = sign * item.Price;
            if (income < 0 && user.Economy.ScamCoins < income)
                return Enums.SellItemResult.NotEnoughMoney;
            user.Economy.ScamCoins += income;
            Database.ItemStats.Remove(item.ItemStats);
            user.Items.Remove(item);
            Database.Items.Remove(item);
            Database.SaveChanges();
            return Enums.SellItemResult.Success;
        }
    }
}