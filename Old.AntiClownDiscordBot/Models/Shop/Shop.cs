using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiClownBot.Helpers;
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

        private readonly Dictionary<int, string> _boughtItemsInfo = new();

        public async Task UpdateShopMessage()
        {
            var newShop = ShopApi.UserShop(UserId);

            var embed = CreateNewShopEmbed(newShop);
            await Message.ModifyAsync(embed);
        }

        public static DiscordEmbed CreateLoadingShopEmbed()
        {
            var loadingEmotes = new List<string>()
            {
                $"{Utility.Emoji(":pigRoll:")}",
                $"{Utility.Emoji(":Applecatrun:")}",
                $"{Utility.Emoji(":SCAMMED:")}",
                $"{Utility.Emoji(":COGGERS:")}",
                $"{Utility.Emoji(":RainbowPls:")}",
                $"{Utility.Emoji(":PolarStrut:")}",
                $"{Utility.Emoji(":popCat:")}",
            };

            var loadingEmbedBuilder = new DiscordEmbedBuilder();
            loadingEmbedBuilder.WithTitle($"Загрузка магазина... {loadingEmotes.SelectRandomItem().Multiply(5)}");

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
            var idResponse = ShopApi.ItemIdInSlot(UserId, slot);
            if (idResponse.HasError)
            {
                await Message.RespondAsync($"{Member.Mention} {idResponse.Error}");
                return;
            }

            var revealResponse = ShopApi.ItemReveal(UserId, idResponse.ShopItemId);

            var responseBuilder = new DiscordMessageBuilder();
            responseBuilder.WithAllowedMention(UserMention.All);
            switch (revealResponse.RevealResult)
            {
                case Enums.RevealResult.Success:
                    break;
                case Enums.RevealResult.NotEnoughMoney:
                    responseBuilder.Content = $"{Member.Mention} недостаточно денег для распознавания предмета";
                    await Message.RespondAsync(responseBuilder);
                    return;
                case Enums.RevealResult.AlreadyRevealed:
                    responseBuilder.Content = $"{Member.Mention} предмет уже распознан";
                    await Message.RespondAsync(responseBuilder);
                    return;
                case Enums.RevealResult.AlreadyBought:
                    responseBuilder.Content = $"{Member.Mention} предмет уже куплен";
                    await Message.RespondAsync(responseBuilder);
                    return;
                case Enums.RevealResult.ItemDoesntExistInShop:
                    responseBuilder.Content = $"{Member.Mention} такого предмета нет в магазине (wtf?)";
                    await Message.RespondAsync(responseBuilder);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await UpdateShopMessage();
        }

        public async void BuyItem(int slot)
        {
            var idResponse = ApiWrapper.Wrappers.ShopApi.ItemIdInSlot(UserId, slot);
            if (idResponse.HasError)
            {
                await Message.RespondAsync($"{Member.Mention} {idResponse.Error}");
                return;
            }

            var buyResponse = ShopApi.Buy(UserId, idResponse.ShopItemId);

            var responseBuilder = new DiscordMessageBuilder();
            responseBuilder.WithAllowedMention(UserMention.All);
            switch (buyResponse.BuyResult)
            {
                case Enums.BuyResult.Success:
                    break;
                case Enums.BuyResult.NotEnoughMoney:
                    responseBuilder.Content = $"{Member.Mention} недостаточно денег для покупки предмета";
                    await Message.RespondAsync(responseBuilder);
                    return;
                case Enums.BuyResult.AlreadyBought:
                    responseBuilder.Content = $"{Member.Mention} предмет уже куплен";
                    await Message.RespondAsync(responseBuilder);
                    return;
                case Enums.BuyResult.ItemDoesntExistInShop:
                    responseBuilder.Content = $"{Member.Mention} такого предмета нет в магазине (wtf?)";
                    await Message.RespondAsync(responseBuilder);
                    return;
                case Enums.BuyResult.TooManyItemsOfSelectedType:
                    responseBuilder.Content =
                        $"{Member.Mention} в инвентаре уже слишком много предметов данного типа (но я это уже сделал по-другому, хз как можно было получить такой ответ)";
                    await Message.RespondAsync(responseBuilder);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var newItemResponse = UsersApi.GetItemById(UserId, buyResponse.ItemId);
            if (newItemResponse.Result != ItemResult.Success)
            {
                responseBuilder.Content = $"{Member.Mention} хуйня {Utility.Emoji(":Starege:")}";
                await Message.RespondAsync(responseBuilder);
                return;
            }

            var info =
                $"{string.Join("\n", newItemResponse.Item.Description().Select(kv => $"{kv.Key}\n\t{kv.Value}"))}";
            _boughtItemsInfo[slot] = info;

            await UpdateShopMessage();
        }

        public async void ReRoll()
        {
            var rerollResult = ShopApi.ReRoll(UserId);

            if (rerollResult.ReRollResult == Enums.ReRollResult.NotEnoughMoney)
            {
                await Message.RespondAsync($"{Member.Mention} недостаточно денег для реролла");
                return;
            }

            _boughtItemsInfo.Clear();
            await Message.ModifyAsync(CreateLoadingShopEmbed());
            await UpdateShopMessage();
        }

        private DiscordEmbed CreateNewShopEmbed(UserShopResponseDto shop)
        {
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle(
                $"Магазин пользователя {Member.ServerOrUserName()} {Utility.Emoji(":PepegaCredit:")} {Utility.Emoji(":PepegaCredit:")} {Utility.Emoji(":PepegaCredit:")}");
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
            {ApiWrapper.Models.Items.Rarity.Epic, DiscordColor.Purple},
            {ApiWrapper.Models.Items.Rarity.Legendary, DiscordColor.Red},
            {ApiWrapper.Models.Items.Rarity.BlackMarket, DiscordColor.Black},
        };

        public static string Help => "Создает новое сообщение с персональным магазином для пользователя" +
                                       "\nМагазин доступен всегда, можно взаимодействовать с персональным сообщением магазина" +
                                       "\nИзначально предметы в магазине скрыты, так что есть риск вслепую покупать предмет" +
                                       "\nПо умолчанию дается 1 бесплатный распознаватель предмета на день, затем распознавание предмета будет стоить 40% от его стоимости" +
                                       $"\nНажатый {Utility.Emoji(":pepeSearching:")} = распознаватель, ненажатый {Utility.Emoji(":pepeSearching:")} = покупка" +
                                       "\nПокупка предметов - через кнопки, соответствующие слотам магазина" +
                                       $"\nДля реролла магазина нажмите {Utility.Emoji(":COGGERS:")}" +
                                       "\nПри переполнении предметов одного типа из инвентаря автоматически удаляется самый плохой по редкости предмет (или случайный из нескольких одинаковой редкости)" +
                                       "\nПо цвету блока сообщения можно определить максимальную редкость одного из предметов в магазине" +
                                       "\nУдачного выбивания новых предметов!";
    }

    public enum Instrument
    {
        Buying,
        Revealing
    }
}