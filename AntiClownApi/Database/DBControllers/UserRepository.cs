using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace AntiClownBotApi.Database.DBControllers
{
    public class UserRepository
    {
        private DatabaseContext Database { get; }

        public UserRepository(DatabaseContext database)
        {
            Database = database;
        }

        public bool IsUserExist(ulong id)
        {
            return Database.Users.Any(user => user.DiscordId == id);
        }

        public DbUser CreateNewUser(ulong id)
        {
            var user = new DbUser()
            {
                DiscordId = id,
                Items = new List<DbItem>(),
                Shop = DbUserShop.GenerateNewShopForUser(id),
                Stats = new DbUserStats()
            };
            user.Economy = new DbUserEconomy()
            {
                User = user,
                NextTribute = DateTime.Now,
                ScamCoins = NumericConstants.DefaultScamCoins,
                Transactions = new List<DbTransaction>(),
                LootBoxes = 0
            };
            Database.Users.Add(user);
            Save();
            return user;
        }

        public void Save()
        {
            Database.SaveChanges();
        }

        public DbUser GetFullUser(ulong id)
        {
            var user = IsUserExist(id)
                ? Database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Items)
                    .Include(u => u.Stats)
                    .Include(u => u.Shop)
                    .First(u => u.DiscordId == id)
                : CreateNewUser(id);
            return user;
        }

        public DbUser GetUserEconomy(ulong id)
        {
            var user = IsUserExist(id)
                ? Database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == id)
                : CreateNewUser(id);
            return user;
        }

        public DbUser GetUserWithEconomyAndItems(ulong id)
        {
            var user = IsUserExist(id)
                ? Database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Items)
                    .ThenInclude(i => i.ItemStats)
                    .First(u => u.DiscordId == id)
                : CreateNewUser(id);
            return user;
        }

        public List<ulong> GetAllUsersIds()
        {
            return Database.Users.Select(user => user.DiscordId).ToList();
        }

        public List<DbUser> GetAllUsers()
        {
            return Database.Users
                .Include(u => u.Economy)
                .Include(u => u.Shop.Items)
                .Include(u => u.Items)
                .ThenInclude(i => i.ItemStats)
                .ToList();
        }

        public List<DbUser> GetAllUsersWithEconomyAndItems()
        {
            return Database.Users
                .Include(u => u.Economy)
                .Include(u => u.Items)
                .ToList();
        }

        public void ChangeUserBalance(ulong id, int ratingDiff, string reason)
        {
            ChangeUserBalanceWithConnection(id, ratingDiff, reason);
        }

        public void ChangeUserBalanceWithConnection(ulong id, int ratingDiff, string reason)
        {
            var user = IsUserExist(id)
                ? Database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == id)
                : CreateNewUser(id);
            user.Economy.ScamCoins += ratingDiff;
            var transaction = new DbTransaction()
            {
                UserEconomy = user.Economy,
                DateTime = DateTime.Now,
                RatingChange = ratingDiff,
                Description = reason
            };
            Database.Transactions.Add(transaction);
            Save();
        }

        public DbUser GetUserWithShop(ulong userId)
        {
            return IsUserExist(userId)
                ? GetAllUsers()
                    .First(u => u.DiscordId == userId)
                : CreateNewUser(userId);
        }

        public void AddItemToUserWithOverflow(ulong userId, DbItem dbItem)
        {
            var user = GetUserWithShop(userId);
            // var itemsOfType = user.Items.Where(i => i.Name == dbItem.Name).ToList();
            // if (itemsOfType.Count == NumericConstants.MaximumItemsOfOneType)
            // {
            //     var itemToDelete = itemsOfType.OrderBy(i => i.Rarity).First();
            //     user.Items.Remove(itemToDelete);
            //     Database.Items.Remove(itemToDelete);
            // }

            dbItem.User = user;
            Database.Items.Add(dbItem);

            Save();
        }

        public void UpdateUserTributeCooldown(ulong userId, int cooldown)
        {
            var user = IsUserExist(userId)
                ? Database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == userId)
                : CreateNewUser(userId);
            user.Economy.NextTribute = DateTime.Now.AddMilliseconds(cooldown);
            Save();
        }

        public DateTime GetUserNextTributeDateTime(ulong userId)
        {
            var user = IsUserExist(userId)
                ? Database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == userId)
                : CreateNewUser(userId);
            return user.Economy.NextTribute;
        }

        public void RemoveCooldowns()
        {
            Database.Users
                .Include(u => u.Economy)
                .ForEach(user => user.Economy.NextTribute = DateTime.Now);
            Save();
        }

        public ulong GetRichestUser()
        {
            return Database.Users
                .Include(u => u.Economy)
                .OrderByDescending(u => u.Economy.ScamCoins).First().DiscordId;
        }

        public ItemResult GetItemById(ulong userId, Guid itemId, out BaseItem item)
        {
            item = null;
            var user = IsUserExist(userId)
                ? Database.Users
                    .Include(u => u.Items)
                    .ThenInclude(i => i.ItemStats)
                    .First(u => u.DiscordId == userId)
                : CreateNewUser(userId);

            var items = user.Items.Where(i => i.Id == itemId).ToList();
            if (!items.Any())
                return ItemResult.ItemNotFound;

            item = BaseItem.FromDbItem(items[0]);
            return ItemResult.Success;
        }

        public Dictionary<Guid, int> UpdateCooldown(List<DbItem> items, ulong discordId)
        {
            var result = new Dictionary<Guid, int>();

            var cooldown = items
                .Where(item => item.Name.Equals(StringConstants.InternetName))
                .SelectMany(item => Enumerable.Repeat(item, item.ItemStats.InternetGigabytes))
                .Aggregate(NumericConstants.DefaultCooldown, (currentCooldown, dbItem) =>
                {
                    var item = (Internet) dbItem;

                    if (Randomizer.GetRandomNumberBetween(0, 100) >= item.Ping)
                        return currentCooldown;

                    if (result.ContainsKey(item.Id))
                    {
                        result[item.Id]++;
                    }
                    else
                    {
                        result.Add(item.Id, 1);
                    }

                    return currentCooldown * (100d - item.Speed) / 100;
                });

            cooldown = items
                .Where(item => item.Name.Equals(StringConstants.JadeRodName))
                .SelectMany(item => Enumerable.Repeat(item, item.ItemStats.JadeRodLength))
                .Aggregate(cooldown, (currentCooldown, dbItem) =>
                {
                    var item = (JadeRod) dbItem;

                    if (Randomizer.GetRandomNumberBetween(0, 100) >= NumericConstants.CooldownIncreaseChanceByOneJade)
                        return currentCooldown;

                    if (result.ContainsKey(item.Id))
                    {
                        result[item.Id]++;
                    }
                    else
                    {
                        result.Add(item.Id, 1);
                    }

                    return currentCooldown * (100d + dbItem.ItemStats.JadeRodThickness) / 100;
                });

            UpdateUserTributeCooldown(discordId, (int) cooldown);
            return result;
        }
    }
}