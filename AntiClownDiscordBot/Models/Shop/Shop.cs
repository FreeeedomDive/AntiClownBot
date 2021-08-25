using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWrapper.Models.Items;
using ApiWrapper.Responses;
using ApiWrapper.Responses.ShopResponses;
using ApiWrapper.Responses.UserCommandResponses;
using ApiWrapper.Wrappers;
using DSharpPlus.Entities;

namespace AntiClownBot.Models.Shop
{
    public class Shop
    {
        public ulong UserId { get; init; }
        public DiscordMessage Message { get; init; }
        public DiscordMember Member { get; init; }
        public Instrument CurrentInstrument { get; set; } = Instrument.Buying;

        private Dictionary<int, string> _boughtItemsInfo = new();

        public async Task UpdateShopMessage()
        {
            var newShop = ShopWrapper.UserShop(UserId);

            var embed = CreateNewShopEmbed(newShop);
            await Message.ModifyAsync(embed);
        }

        public static DiscordEmbed CreateLoadingEmbed()
        {
            var loadingEmotes = new List<string>()
            {
                $"{Utility.Emoji(":pigRoll:")}" +
                $"{Utility.Emoji(":pigRoll:")}" +
                $"{Utility.Emoji(":pigRoll:")}" +
                $"{Utility.Emoji(":pigRoll:")}" +
                $"{Utility.Emoji(":pigRoll:")}",

                $"{Utility.Emoji(":Applecatrun:")}" +
                $"{Utility.Emoji(":Applecatrun:")}" +
                $"{Utility.Emoji(":Applecatrun:")}" +
                $"{Utility.Emoji(":Applecatrun:")}" +
                $"{Utility.Emoji(":Applecatrun:")}",

                $"{Utility.Emoji(":SCAMMED:")}" +
                $"{Utility.Emoji(":SCAMMED:")}" +
                $"{Utility.Emoji(":SCAMMED:")}" +
                $"{Utility.Emoji(":SCAMMED:")}" +
                $"{Utility.Emoji(":SCAMMED:")}",

                $"{Utility.Emoji(":COGGERS:")}" +
                $"{Utility.Emoji(":COGGERS:")}" +
                $"{Utility.Emoji(":COGGERS:")}" +
                $"{Utility.Emoji(":COGGERS:")}" +
                $"{Utility.Emoji(":COGGERS:")}",

                $"{Utility.Emoji(":RainbowPls:")}" +
                $"{Utility.Emoji(":RainbowPls:")}" +
                $"{Utility.Emoji(":RainbowPls:")}" +
                $"{Utility.Emoji(":RainbowPls:")}" +
                $"{Utility.Emoji(":RainbowPls:")}",
                
                $"{Utility.Emoji(":PolarStrut:")}" +
                $"{Utility.Emoji(":PolarStrut:")}" +
                $"{Utility.Emoji(":PolarStrut:")}" +
                $"{Utility.Emoji(":PolarStrut:")}" +
                $"{Utility.Emoji(":PolarStrut:")}",
                
                $"{Utility.Emoji(":popCat:")}" +
                $"{Utility.Emoji(":popCat:")}" +
                $"{Utility.Emoji(":popCat:")}" +
                $"{Utility.Emoji(":popCat:")}" +
                $"{Utility.Emoji(":popCat:")}",
            };
            
            var loadingEmbedBuilder = new DiscordEmbedBuilder();
            loadingEmbedBuilder.WithTitle($"Загрузка магазина... {loadingEmotes.SelectRandomItem()}");

            return loadingEmbedBuilder.Build();
        }

        public void HandleItemInSlot(int slot)
        {
            switch (CurrentInstrument)
            {
                case Instrument.Buying:
                    BuyItem(slot);
                    return;
                case Instrument.Revealing:
                    RevealItem(slot);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async void RevealItem(int slot)
        {
            var idResponse = ShopWrapper.ItemIdInSlot(UserId, slot);
            if (idResponse.HasError)
            {
                await Message.RespondAsync($"{Member.Mention} {idResponse.Error}");
                return;
            }

            var revealResponse = ShopWrapper.ItemReveal(UserId, idResponse.ShopItemId);

            switch (revealResponse.RevealResult)
            {
                case Enums.RevealResult.Success:
                    break;
                case Enums.RevealResult.NotEnoughMoney:
                    await Message.RespondAsync($"{Member.Mention} недостаточно денег для распознавания предмета");
                    return;
                case Enums.RevealResult.AlreadyRevealed:
                    await Message.RespondAsync($"{Member.Mention} предмет уже распознан");
                    return;
                case Enums.RevealResult.AlreadyBought:
                    await Message.RespondAsync($"{Member.Mention} предмет уже куплен");
                    return;
                case Enums.RevealResult.ItemDoesntExistInShop:
                    await Message.RespondAsync($"{Member.Mention} такого предмета нет в магазине (wtf?)");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await UpdateShopMessage();
        }

        public async void BuyItem(int slot)
        {
            var idResponse = ApiWrapper.Wrappers.ShopWrapper.ItemIdInSlot(UserId, slot);
            if (idResponse.HasError)
            {
                await Message.RespondAsync($"{Member.Mention} {idResponse.Error}");
                return;
            }

            var buyResponse = ApiWrapper.Wrappers.ShopWrapper.Buy(UserId, idResponse.ShopItemId);

            switch (buyResponse.BuyResult)
            {
                case Enums.BuyResult.Success:
                    break;
                case Enums.BuyResult.NotEnoughMoney:
                    await Message.RespondAsync($"{Member.Mention} недостаточно денег для покупки предмета");
                    return;
                case Enums.BuyResult.AlreadyBought:
                    await Message.RespondAsync($"{Member.Mention} предмет уже куплен");
                    return;
                case Enums.BuyResult.ItemDoesntExistInShop:
                    await Message.RespondAsync($"{Member.Mention} такого предмета нет в магазине (wtf?)");
                    return;
                case Enums.BuyResult.TooManyItemsOfSelectedType:
                    await Message.RespondAsync(
                        $"{Member.Mention} в инвентаре уже слишком много предметов данного типа (но я это уже сделал по-другому, хз как можно было получить такой ответ)");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newItemResponse = ApiWrapper.Wrappers.UsersWrapper.GetItemById(UserId, buyResponse.ItemId);
            if (newItemResponse.Result != ItemResult.Success)
            {
                await Message.RespondAsync($"{Member.Mention} хуйня {Utility.Emoji(":Starege:")}");
                return;
            }

            var info =
                $"{string.Join("\n", newItemResponse.Item.Description().Select(kv => $"{kv.Key}\n\t{kv.Value}"))}";
            _boughtItemsInfo[slot] = info;

            await UpdateShopMessage();
        }

        public async void ReRoll()
        {
            var rerollResult = ApiWrapper.Wrappers.ShopWrapper.ReRoll(UserId);

            if (rerollResult.ReRollResult == Enums.ReRollResult.NotEnoughMoney)
            {
                await Message.RespondAsync($"{Member.Mention} недостаточно денег для реролла");
                return;
            }

            _boughtItemsInfo.Clear();
            await Message.ModifyAsync(CreateLoadingEmbed());
            await UpdateShopMessage();
        }

        private DiscordEmbed CreateNewShopEmbed(UserShopResponseDto shop)
        {
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle(
                $"Магазин пользователя {Member.Nickname} {Utility.Emoji(":PepegaCredit:")} {Utility.Emoji(":PepegaCredit:")} {Utility.Emoji(":PepegaCredit:")}");
            embedBuilder.AddField("Баланс", $"{shop.Balance}", true);
            embedBuilder.AddField("Цена реролла магазина", $"{shop.ReRollPrice}", true);
            embedBuilder.AddField("Распознавание предмета", $"{shop.FreeItemReveals}", true);
            var itemIndex = 1;
            var maxRarity = shop.Items.OrderByDescending(item => item.Rarity).First().Rarity;
            embedBuilder.WithColor(Color[maxRarity]);
            foreach (var shopItem in shop.Items)
            {
                var itemContent = shopItem.IsOwned
                    ? "КУПЛЕН" + (_boughtItemsInfo.ContainsKey(itemIndex) ? $"\n{_boughtItemsInfo[itemIndex]}" : "")
                    : $"Редкость: {Rarity[shopItem.Rarity]}\n" +
                      $"Цена: {shopItem.Price}";
                embedBuilder.AddField(
                    $"{itemIndex}. " + (shopItem.IsRevealed ? shopItem.Name :
                        shopItem.IsOwned ? shopItem.Name : "Нераспознанный предмет"),
                    itemContent);
                itemIndex++;
            }

            return embedBuilder.Build();
        }

        private static readonly Dictionary<Rarity, string> Rarity = new()
        {
            {ApiWrapper.Models.Items.Rarity.Common, "Обычная"},
            {ApiWrapper.Models.Items.Rarity.Rare, "Редкая"},
            {ApiWrapper.Models.Items.Rarity.Epic, "Эпическая"},
            {ApiWrapper.Models.Items.Rarity.Legendary, "Легендарная"},
            {ApiWrapper.Models.Items.Rarity.BlackMarket, "С черного рынка"},
        };

        private static readonly Dictionary<Rarity, DiscordColor> Color = new()
        {
            {ApiWrapper.Models.Items.Rarity.Common, DiscordColor.Gray},
            {ApiWrapper.Models.Items.Rarity.Rare, DiscordColor.Blue},
            {ApiWrapper.Models.Items.Rarity.Epic, DiscordColor.Violet},
            {ApiWrapper.Models.Items.Rarity.Legendary, DiscordColor.Orange},
            {ApiWrapper.Models.Items.Rarity.BlackMarket, DiscordColor.Magenta},
        };
    }

    public enum Instrument
    {
        Buying,
        Revealing
    }
}