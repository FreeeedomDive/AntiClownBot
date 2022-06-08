using AntiClownApiClient;
using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses;
using AntiClownApiClient.Dto.Responses.ShopResponses;
using AntiClownApiClient.Dto.Responses.UserCommandResponses;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Models.Shop
{
    public class Shop
    {
        public Shop(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.randomizer = randomizer;
        }

        public async Task<DiscordEmbed> GetNewShopEmbed()
        {
            var newShop = await apiClient.Shop.GetAsync(UserId);

            return await CreateNewShopEmbed(newShop);
        }

        public async Task<DiscordEmbed> CreateLoadingShopEmbed()
        {
            var loadingEmotes = new List<string>()
            {
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("pigRoll")}",
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("Applecatrun")}",
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("SCAMMED")}",
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("COGGERS")}",
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("RainbowPls")}",
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("PolarStrut")}",
                $"{await discordClientWrapper.Emotes.FindEmoteAsync("popCat")}",
            };

            var loadingEmbedBuilder = new DiscordEmbedBuilder();
            loadingEmbedBuilder.WithTitle($"Загрузка магазина... {loadingEmotes.SelectRandomItem(randomizer).Multiply(5)}");

            return loadingEmbedBuilder.Build();
        }

        public async Task HandleItemInSlot(int slot, DiscordInteraction interaction)
        {
            switch (CurrentShopTool)
            {
                case ShopTool.Buying:
                    await BuyItem(slot, interaction);
                    return;
                case ShopTool.Revealing:
                    await RevealItem(slot, interaction);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task RevealItem(int slot, DiscordInteraction interaction)
        {
            var idResponse = await apiClient.Shop.ItemIdInSlotAsync(UserId, slot);
            var responseBuilder = new DiscordInteractionResponseBuilder();
            if (idResponse.HasError)
            {
                responseBuilder.WithContent($"{Member.Mention} {idResponse.Error}");
                await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.UpdateMessage, responseBuilder);
                return;
            }

            var revealResponse = await apiClient.Shop.ItemRevealAsync(UserId, idResponse.ShopItemId);

            switch (revealResponse.RevealResult)
            {
                case Enums.RevealResult.Success:
                    break;
                case Enums.RevealResult.NotEnoughMoney:
                    responseBuilder.WithContent($"{Member.Mention} недостаточно денег для распознавания предмета");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.UpdateMessage, responseBuilder);
                    return;
                case Enums.RevealResult.AlreadyRevealed:
                    responseBuilder.WithContent($"{Member.Mention} предмет уже распознан");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.UpdateMessage, responseBuilder);
                    return;
                case Enums.RevealResult.AlreadyBought:
                    responseBuilder.WithContent($"{Member.Mention} предмет уже куплен");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.UpdateMessage, responseBuilder);
                    return;
                case Enums.RevealResult.ItemDoesntExistInShop:
                    responseBuilder.WithContent($"{Member.Mention} такого предмета нет в магазине (wtf?)");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.UpdateMessage, responseBuilder);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task BuyItem(int slot, DiscordInteraction interaction)
        {
            var idResponse = await apiClient.Shop.ItemIdInSlotAsync(UserId, slot);
            var responseBuilder = new DiscordInteractionResponseBuilder();
            if (idResponse.HasError)
            {
                await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder.WithContent($"{Member.Mention} {idResponse.Error}"));
                return;
            }

            var buyResponse = await apiClient.Shop.BuyAsync(UserId, idResponse.ShopItemId);

            switch (buyResponse.BuyResult)
            {
                case Enums.BuyResult.Success:
                    break;
                case Enums.BuyResult.NotEnoughMoney:
                    responseBuilder.WithContent($"{Member.Mention} недостаточно денег для покупки предмета");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder);
                    return;
                case Enums.BuyResult.AlreadyBought:
                    responseBuilder.WithContent($"{Member.Mention} предмет уже куплен");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder);
                    return;
                case Enums.BuyResult.ItemDoesntExistInShop:
                    responseBuilder.WithContent($"{Member.Mention} такого предмета нет в магазине (wtf?)");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder);
                    return;
                case Enums.BuyResult.TooManyItemsOfSelectedType:
                    // xdd
                    responseBuilder.WithContent($"{Member.Mention} в инвентаре уже слишком много предметов данного типа (но я это уже сделал по-другому, хз как можно было получить такой ответ)");
                    await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newItemResponse = await apiClient.Users.GetItemByIdAsync(UserId, buyResponse.ItemId);
            if (newItemResponse.Result != ItemResult.Success)
            {
                var starege = await discordClientWrapper.Emotes.FindEmoteAsync("Starege");
                responseBuilder.WithContent($"{Member.Mention} хуйня {starege}");
                await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder);
                return;
            }

            var info = $"{string.Join("\n", newItemResponse.Item.Description().Select(kv => $"{kv.Key}\n\t{kv.Value}"))}";
            boughtItemsInfo[slot] = info;
        }

        public async Task ReRoll(DiscordInteraction interaction)
        {
            var rerollResult = await apiClient.Shop.ReRollAsync(UserId);
            var responseBuilder = new DiscordInteractionResponseBuilder();

            if (rerollResult.ReRollResult == Enums.ReRollResult.NotEnoughMoney)
            {
                await discordClientWrapper.Messages.RespondAsync(interaction, InteractionResponseType.ChannelMessageWithSource, responseBuilder.WithContent($"{Member.Mention} недостаточно денег для реролла"));
                return;
            }

            boughtItemsInfo.Clear();
        }

        private async Task<DiscordEmbed> CreateNewShopEmbed(UserShopResponseDto shop)
        {
            var pepegaCredit = await discordClientWrapper.Emotes.FindEmoteAsync("PepegaCredit");
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle(
                $"Магазин пользователя {Member.ServerOrUserName()} {pepegaCredit} {pepegaCredit} {pepegaCredit}");
            embedBuilder.AddField("Баланс", $"{shop.Balance}", true);
            embedBuilder.AddField("Цена реролла магазина", $"{shop.ReRollPrice}", true);
            embedBuilder.AddField("Распознавание предмета", $"{shop.FreeItemReveals}", true);
            var itemIndex = 1;
            var maxRarity = shop.Items.OrderByDescending(item => item.Rarity).First().Rarity;
            embedBuilder.WithColor(Color[maxRarity]);
            foreach (var shopItem in shop.Items)
            {
                var itemContent = shopItem.IsOwned
                    ? "КУПЛЕН" + (boughtItemsInfo.ContainsKey(itemIndex) ? $"\n{boughtItemsInfo[itemIndex]}" : "")
                    : $"Редкость: {Rarity[shopItem.Rarity]}\n" +
                      $"Цена: {shopItem.Price}";
                embedBuilder.AddField(
                    $"{itemIndex}. " + (shopItem.IsRevealed ? shopItem.Name :
                        shopItem.IsOwned ? shopItem.Name : "Нераспознанный предмет"),
                    itemContent);
                itemIndex++;
            }

            embedBuilder.WithFooter($"Текущее действие: {StringTool()}");

            return embedBuilder.Build();
        }

        private static readonly Dictionary<Rarity, string> Rarity = new()
        {
            {AntiClownApiClient.Dto.Models.Items.Rarity.Common, "Обычная"},
            {AntiClownApiClient.Dto.Models.Items.Rarity.Rare, "Редкая"},
            {AntiClownApiClient.Dto.Models.Items.Rarity.Epic, "Эпическая"},
            {AntiClownApiClient.Dto.Models.Items.Rarity.Legendary, "Легендарная"},
            {AntiClownApiClient.Dto.Models.Items.Rarity.BlackMarket, "С черного рынка"},
        };

        private static readonly Dictionary<Rarity, DiscordColor> Color = new()
        {
            {AntiClownApiClient.Dto.Models.Items.Rarity.Common, DiscordColor.Gray},
            {AntiClownApiClient.Dto.Models.Items.Rarity.Rare, DiscordColor.Blue},
            {AntiClownApiClient.Dto.Models.Items.Rarity.Epic, DiscordColor.Purple},
            {AntiClownApiClient.Dto.Models.Items.Rarity.Legendary, DiscordColor.Red},
            {AntiClownApiClient.Dto.Models.Items.Rarity.BlackMarket, DiscordColor.Black},
        };

        private string StringTool() => CurrentShopTool switch
        {
            ShopTool.Buying => "покупка",
            ShopTool.Revealing => "пупа и лупа",
            _ => throw new ArgumentOutOfRangeException()
        };

        public ulong UserId { get; init; }
        public DiscordMember Member { get; init; }
        public ShopTool CurrentShopTool { get; set; } = ShopTool.Buying;

        private readonly Dictionary<int, string> boughtItemsInfo = new();
        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IRandomizer randomizer;
    }
}