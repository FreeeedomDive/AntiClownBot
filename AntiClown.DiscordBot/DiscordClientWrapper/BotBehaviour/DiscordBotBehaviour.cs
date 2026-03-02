using System.Text;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Ai.Client;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Interactivity.Domain.Inventory;
using AntiClown.DiscordBot.Interactivity.Domain.Shop;
using AntiClown.DiscordBot.Interactivity.Services.GuessNumber;
using AntiClown.DiscordBot.Interactivity.Services.Inventory;
using AntiClown.DiscordBot.Interactivity.Services.Lottery;
using AntiClown.DiscordBot.Interactivity.Services.Parsers;
using AntiClown.DiscordBot.Interactivity.Services.Parties;
using AntiClown.DiscordBot.Interactivity.Services.Race;
using AntiClown.DiscordBot.Interactivity.Services.Shop;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Achievements;
using AntiClown.DiscordBot.SlashCommands.Dev;
using AntiClown.DiscordBot.SlashCommands.F1Predictions;
using AntiClown.DiscordBot.SlashCommands.Gaming;
using AntiClown.DiscordBot.SlashCommands.Inventory;
using AntiClown.DiscordBot.SlashCommands.MinecraftRegister;
using AntiClown.DiscordBot.SlashCommands.Other;
using AntiClown.DiscordBot.SlashCommands.Other.Events;
using AntiClown.DiscordBot.SlashCommands.Other.Race;
using AntiClown.DiscordBot.SlashCommands.Random;
using AntiClown.DiscordBot.SlashCommands.Roles;
using AntiClown.DiscordBot.SlashCommands.SocialRating;
using AntiClown.DiscordBot.SlashCommands.Voice;
using AntiClown.DiscordBot.SlashCommands.Web;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Tools.Utility.Extensions;
using AntiClown.Tools.Utility.Random;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;

public class DiscordBotBehaviour(
    IServiceProvider serviceProvider,
    DiscordClient discordClient,
    IDiscordClientWrapper discordClientWrapper,
    IEmotesCache emotesCache,
    IAntiClownDataApiClient antiClownDataApiClient,
    IInventoryService inventoryService,
    IShopService shopService,
    IGuessNumberEventService guessNumberEventService,
    ILotteryService lotteryService,
    IPartiesService partiesService,
    ILogger<DiscordBotBehaviour> logger,
    IRaceService raceService,
    IAiClient aiClient
) : IDiscordBotBehaviour
{
    public async Task ConfigureAsync()
    {
        discordClient.GuildEmojisUpdated += GuildEmojisUpdated;
        discordClient.MessageCreated += MessageCreated;
        discordClient.MessageReactionAdded += MessageReactionAdded;
        discordClient.ComponentInteractionCreated += ComponentInteractionCreated;

        await RegisterSlashCommandsAsync(discordClient);
    }

    private async Task GuildEmojisUpdated(DiscordClient _, GuildEmojisUpdateEventArgs e)
    {
        var isEmoteNotificationEnabled = await antiClownDataApiClient.Settings.ReadBoolAsync(SettingsCategory.DiscordBot, "IsEmoteNotificationEnabled");
        if (!isEmoteNotificationEnabled)
        {
            return;
        }

        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        if (e.EmojisAfter.Count > e.EmojisBefore.Count)
        {
            var messageBuilder = new StringBuilder(
                $"{await emotesCache.GetEmoteAsTextAsync("pepeLaugh")} " +
                $"{await emotesCache.GetEmoteAsTextAsync("point_right")}"
            );
            foreach (var (key, emoji) in e.EmojisAfter)
            {
                if (!e.EmojisBefore.ContainsKey(key))
                {
                    messageBuilder.Append($" {emoji}");
                }
            }

            await discordClientWrapper.Messages.SendAsync(
                botChannelId,
                messageBuilder.ToString()
            );
            return;
        }

        if (e.EmojisBefore.Count > e.EmojisAfter.Count)
        {
            var messageBuilder = new StringBuilder();
            foreach (var (key, emoji) in e.EmojisBefore)
            {
                if (!e.EmojisAfter.ContainsKey(key))
                {
                    messageBuilder.Append($"{emoji} ");
                }
            }

            messageBuilder.Append($"удалили {await emotesCache.GetEmoteAsTextAsync("BibleThump")}");
            await discordClientWrapper.Messages.SendAsync(
                botChannelId,
                messageBuilder.ToString()
            );
        }
    }

    private async Task MessageCreated(DiscordClient _, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot)
        {
            return;
        }

        var message = e.Message.Content;

        var partyChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "PartyChannelId");
        // удаляем все сообщения из чата с пати, чтобы люди отвечали в треды
        if (e.Channel.Id == partyChannelId)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                               .WithTitle("Модерация чата пати")
                               .WithColor(DiscordColor.DarkRed)
                               .AddField(
                                   "Модерация чата пати",
                                   "Если нужно собрать пати, воспользуйся слеш-командой /party\n" +
                                   "Если нужно ответить по какому-то пати, сделай это в соответствующем треде"
                               );
            var messageBuilder = new DiscordMessageBuilder()
                                 .WithEmbed(embedBuilder.Build())
                                 .WithAllowedMentions(Mentions.All)
                                 .WithContent(e.Author.Mention);
#pragma warning disable CS4014 // Не дожидаемся выполнения этого потока
            Task.Run(
                async () =>
                {
                    var response = await discordClientWrapper.Messages.RespondAsync(e.Message, messageBuilder);
                    await discordClientWrapper.Messages.DeleteAsync(e.Message);
                    await Task.Delay(10000);
                    await discordClientWrapper.Messages.DeleteAsync(response);
                }
            );
#pragma warning restore CS4014
            return;
        }

        message = message.ToLower();

        if (message.Length > 0 && message[^1] == '?')
        {
            if (message.Contains("когда"))
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "Завтра в 3");
                return;
            }

            if ((message.Contains("бот, ты") || message.Contains("бот ты")) &&
                Randomizer.GetRandomNumberBetween(0, 3) == 0)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "А может ты?");
                return;
            }

            if (Randomizer.GetRandomNumberBetween(0, 10) == 7)
            {
                var aiResponse = await aiClient.GetResponseAsync(message);
                await discordClientWrapper.Messages.RespondAsync(
                    e.Message,
                    aiResponse
                );
            }
            else
            {
                if (Randomizer.CoinFlip())
                {
                    await discordClientWrapper.Messages.RespondAsync(
                        e.Message,
                        $"{await emotesCache.GetEmoteAsTextAsync("YEP")}"
                    );
                }
                else
                {
                    await discordClientWrapper.Messages.RespondAsync(
                        e.Message,
                        $"{await emotesCache.GetEmoteAsTextAsync("NOPE")}"
                    );
                }
            }

            return;
        }

        var botId = await discordClientWrapper.Members.GetBotIdAsync();
        if (message.Contains($"<@!{botId}>"))
        {
            await ReactToAppeal(e.Message);
            return;
        }

        var cocks = IsCockInMessage(message);
        var booba = IsBoobaInMessage(message);
        var anime = IsAnimeInMessage(message);

        if ((cocks || booba || anime) && !(message.Contains("youtu") || message.Contains("spotify")))
        {
            if (cocks)
            {
                var didSomeoneSayCock = await emotesCache.GetEmoteAsync("DIDSOMEONESAYCOCK");
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(e.Message, didSomeoneSayCock);
                var yep = await emotesCache.GetEmoteAsTextAsync("YEP");
                await discordClientWrapper.Messages.RespondAsync(e.Message, $"{yep} COCK");
            }

            if (booba)
            {
                if (Randomizer.CoinFlip())
                {
                    await discordClientWrapper.Messages.RespondAsync(
                        e.Message,
                        "https://tenor.com/view/booba-boobs-coom-hecute-pepe-gif-19186230"
                    );
                }
                else
                {
                    await discordClientWrapper.Messages.RespondAsync(
                        e.Message,
                        "https://tenor.com/view/booba-pepe-meme-4chan-huypenis-gif-18858228"
                    );
                }
            }

            if (anime)
            {
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(
                    e.Message,
                    await emotesCache.GetEmoteAsTextAsync("AYAYABASS")
                );
            }
        }

        var xddAnswersEnabled = await antiClownDataApiClient.Settings.ReadBoolAsync(SettingsCategory.DiscordBot, "XddAnswersEnabled");
        if (xddAnswersEnabled && Randomizer.GetRandomNumberBetween(0, 25) == 0)
        {
            var xddPool = new[]
            {
                "xdd",
                "xdd666",
                "xpp",
                "xddx",
                "xddnerd",
                "xddshy",
                "xddDespair",
                "xddfdhjsd0f76ds5r26FDSFHD88hjdbs",
                "xddktulhu",
                "xddxdd",
                "amongdd",
                "ddx",
                "xdd258069758",
                "xdd53153120598203958230958259038",
                "xddsittingverycomfortable",
                "xddriki",
                "odpuzzle",
                "xddWHAT",
                "xddJam",
                "xddChatting",
            };
            var xdd = await emotesCache.GetEmoteAsTextAsync(xddPool.SelectRandomItem());
            await discordClientWrapper.Messages.RespondAsync(e.Message, xdd);
        }
    }

    private async Task MessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
    {
        if (e.User.IsBot)
        {
            return;
        }

        var emoji = e.Emoji;
        var emojiName = emoji.Name;

        if (emojiName is "peepoClown" or "clown" or "clown_face"
            && e.User.Id is 423498706336088085 or 369476500820459522)
        {
            await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
        }
    }

    private async Task ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs eventArgs)
    {
        try
        {
            if (eventArgs.Id.StartsWith(InteractionsIds.InventoryButtons.Prefix))
            {
                await HandleInventoryInteractionAsync(eventArgs);
                return;
            }

            if (eventArgs.Id.StartsWith(InteractionsIds.ShopButtons.Prefix))
            {
                await HandleShopInteractionAsync(eventArgs);
                return;
            }

            if (eventArgs.Id.StartsWith(InteractionsIds.EventsButtons.GuessNumber.Prefix))
            {
                await HandleGuessNumberInteractionAsync(eventArgs);
            }

            if (eventArgs.Id.StartsWith(InteractionsIds.EventsButtons.Lottery.Prefix))
            {
                await HandleLotteryInteractionAsync(eventArgs);
            }

            if (eventArgs.Id.StartsWith(InteractionsIds.EventsButtons.Race.Prefix))
            {
                await HandleRaceInteractionAsync(eventArgs);
            }

            if (eventArgs.Id.StartsWith(InteractionsIds.PartyButtons.Prefix))
            {
                await HandlePartyInteractionAsync(eventArgs);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception in component interaction {InteractionId} by member {MemberId}",
                eventArgs.Id,
                eventArgs.User.Id
            );
        }
    }

    private async Task ValidateInteractionUserAsync(ComponentInteractionCreateEventArgs e)
    {
        var interactionAuthor = e.Message.Interaction?.User;
        var responseBuilder = new DiscordInteractionResponseBuilder();
        if (interactionAuthor == null || e.User.Id != interactionAuthor.Id)
        {
            var member = await discordClientWrapper.Members.GetAsync(e.User.Id);
            await discordClientWrapper.Messages.RespondAsync(
                e.Interaction,
                InteractionResponseType.ChannelMessageWithSource,
                responseBuilder.WithContent(
                    $"{member.Mention} " +
                    $"НЕ НАДО ЮЗАТЬ ЧУЖИЕ КНОПКИ " +
                    $"{await emotesCache.GetEmoteAsTextAsync("Madge")}"
                )
            );
        }
    }

    private async Task HandleShopInteractionAsync(ComponentInteractionCreateEventArgs e)
    {
        await ValidateInteractionUserAsync(e);
        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );

        var (id, action) = ShopButtonsParser.Parse(e.Id);

        switch (action)
        {
            case InteractionsIds.ShopButtons.ShopButtonItem1:
                await shopService.HandleItemInSlotAsync(id, 0, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonItem2:
                await shopService.HandleItemInSlotAsync(id, 1, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonItem3:
                await shopService.HandleItemInSlotAsync(id, 2, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonItem4:
                await shopService.HandleItemInSlotAsync(id, 3, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonItem5:
                await shopService.HandleItemInSlotAsync(id, 4, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonReroll:
                await shopService.ReRollAsync(id, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonBuy:
                await shopService.SetActiveToolAsync(id, ShopTool.Buying, UpdateMessageAsync);
                break;
            case InteractionsIds.ShopButtons.ShopButtonReveal:
                await shopService.SetActiveToolAsync(id, ShopTool.Revealing, UpdateMessageAsync);
                break;
        }

        return;

        Task UpdateMessageAsync(DiscordWebhookBuilder webhookBuilder)
        {
            return discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction, webhookBuilder);
        }
    }

    private async Task HandleInventoryInteractionAsync(ComponentInteractionCreateEventArgs e)
    {
        await ValidateInteractionUserAsync(e);
        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );

        var (id, action) = InventoryButtonsParser.Parse(e.Id);
        switch (action)
        {
            case InteractionsIds.InventoryButtons.InventoryButton1:
                await inventoryService.HandleItemInSlotAsync(id, 0, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButton2:
                await inventoryService.HandleItemInSlotAsync(id, 1, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButton3:
                await inventoryService.HandleItemInSlotAsync(id, 2, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButton4:
                await inventoryService.HandleItemInSlotAsync(id, 3, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButton5:
                await inventoryService.HandleItemInSlotAsync(id, 4, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButtonLeft:
                await inventoryService.ChangePageAsync(id, -1, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButtonRight:
                await inventoryService.ChangePageAsync(id, 1, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButtonChangeActiveStatus:
                await inventoryService.SetActiveToolAsync(id, InventoryTool.ChangeActiveStatus, UpdateMessageAsync);
                break;
            case InteractionsIds.InventoryButtons.InventoryButtonSell:
                await inventoryService.SetActiveToolAsync(id, InventoryTool.Sell, UpdateMessageAsync);
                break;
        }

        return;

        Task UpdateMessageAsync(DiscordWebhookBuilder webhookBuilder)
        {
            return discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction, webhookBuilder);
        }
    }

    private async Task HandleGuessNumberInteractionAsync(ComponentInteractionCreateEventArgs e)
    {
        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );
        var (eventId, pick) = GuessNumberButtonsParser.Parse(e.Id);
        await guessNumberEventService.AddUserPickAsync(eventId, e.User.Id, (GuessNumberPickDto)pick);
    }

    private async Task HandleLotteryInteractionAsync(ComponentInteractionCreateEventArgs e)
    {
        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );
        var (eventId, action) = LotteryButtonsParser.Parse(e.Id);
        switch (action)
        {
            case InteractionsIds.EventsButtons.Lottery.Join:
                await lotteryService.AddParticipantAsync(eventId, e.User.Id);
                break;
        }
    }

    private async Task HandleRaceInteractionAsync(ComponentInteractionCreateEventArgs e)
    {
        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );
        var (eventId, action) = RaceButtonsParser.Parse(e.Id);
        switch (action)
        {
            case InteractionsIds.EventsButtons.Race.Join:
                await raceService.AddParticipantAsync(eventId, e.User.Id);
                break;
        }
    }

    private async Task HandlePartyInteractionAsync(ComponentInteractionCreateEventArgs e)
    {
        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );
        var (eventId, action) = PartyButtonsParser.Parse(e.Id);
        switch (action)
        {
            case InteractionsIds.PartyButtons.Join:
                await partiesService.AddPlayerAsync(eventId, e.User.Id);
                break;
            case InteractionsIds.PartyButtons.Leave:
                await partiesService.RemovePlayerAsync(eventId, e.User.Id);
                break;
            case InteractionsIds.PartyButtons.Close:
                await partiesService.ClosePartyAsync(eventId, e.User.Id);
                break;
            case InteractionsIds.PartyButtons.Ping:
                await partiesService.PingReadyPlayersAsync(eventId, e.User.Id);
                break;
        }
    }

    private async Task RegisterSlashCommandsAsync(DiscordClient client)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var slash = client.UseSlashCommands(
            new SlashCommandsConfiguration
            {
                Services = serviceProvider,
            }
        );
        slash.RegisterCommands<MinecraftRegisterCommand>(guildId);
        slash.RegisterCommands<PartyCommandModule>(guildId);
        slash.RegisterCommands<InventoryCommandModule>(guildId);
        slash.RegisterCommands<LootBoxCommandModule>(guildId);
        slash.RegisterCommands<ShopCommandModule>(guildId);
        slash.RegisterCommands<LohotronCommandModule>(guildId);
        slash.RegisterCommands<RatingCommandModule>(guildId);
        slash.RegisterCommands<RolesCommandModule>(guildId);
        slash.RegisterCommands<ChangeNicknameCommandModule>(guildId);
        slash.RegisterCommands<RaceCommandModule>(guildId);
        slash.RegisterCommands<TributeCommandModule>(guildId);
        slash.RegisterCommands<WhenCommandModule>(guildId);
        slash.RegisterCommands<TransactionsCommandModule>(guildId);
        slash.RegisterCommands<RollCommandModule>(guildId);
        slash.RegisterCommands<SelectCommandModule>(guildId);
        slash.RegisterCommands<F1CommandModule>(guildId);
        slash.RegisterCommands<F1StatsCommand>(guildId);
        slash.RegisterCommands<WebCommandModule>(guildId);
        slash.RegisterCommands<VoiceAiCommandModule>(guildId);
        slash.RegisterCommands<AiCommandModule>(guildId);

        // admin commands
        slash.RegisterCommands<UserSocialRatingEditorCommandModule>(guildId);
        slash.RegisterCommands<DailyResetCommandModule>(guildId);
        slash.RegisterCommands<RefreshUsersCacheCommandModule>(guildId);
        slash.RegisterCommands<RightsCommandModule>(guildId);
        slash.RegisterCommands<CreateMessageCommandModule>(guildId);
        slash.RegisterCommands<EventsCommandModule>(guildId);
        slash.RegisterCommands<F1AdminCommandModule>(guildId);
        slash.RegisterCommands<RolesAdminCommandModule>(guildId);
        slash.RegisterCommands<UsersCommandModule>(guildId);
        slash.RegisterCommands<AchievementsAdminCommandModule>(guildId);
    }

    private async Task ReactToAppeal(DiscordMessage message)
    {
        var pool = new[]
        {
            await emotesCache.GetEmoteAsTextAsync("PogOff"),
            await emotesCache.GetEmoteAsTextAsync("gachiBASS"),
            await emotesCache.GetEmoteAsTextAsync("monkaW"),
            await emotesCache.GetEmoteAsTextAsync("xdd"),
            await emotesCache.GetEmoteAsTextAsync("ok"),
            await emotesCache.GetEmoteAsTextAsync("SHTOOO"),
            await emotesCache.GetEmoteAsTextAsync("Starege"),
            await emotesCache.GetEmoteAsTextAsync("peepoPooPoo"),
            await emotesCache.GetEmoteAsTextAsync("peepoRip"),
            await emotesCache.GetEmoteAsTextAsync("aRolf"),
        };
        await discordClientWrapper.Messages.RespondAsync(
            message,
            pool[Randomizer.GetRandomNumberBetween(0, pool.Length)]
        );
    }

    private static bool IsCockInMessage(string message)
    {
        var symbols = new[] { 'c', 'o', 'k' };
        var correctMessage = string
            .Join(
                "",
                message
                    .ToCharArray()
                    .Where(ch => symbols.Contains(ch))
            );
        var sb = new StringBuilder();
        if (correctMessage.Length == 0)
        {
            return false;
        }

        sb.Append(correctMessage[0]);
        for (var i = 1; i < correctMessage.Length; i++)
        {
            var ch = correctMessage[i];
            if (ch != correctMessage[i - 1])
            {
                sb.Append(ch);
            }
        }

        var result = sb.ToString();
        return result.Contains("cock");
    }

    private static bool IsBoobaInMessage(string message)
    {
        var symbols = new[] { 'b', 'o' };
        var correctMessage = string.Join(
            "",
            message
                .ToCharArray()
                .Where(ch => symbols.Contains(ch))
        );
        return correctMessage.Contains("boob");
    }

    private static bool IsAnimeInMessage(string message)
    {
        var symbols = new[] { 'a', 'n', 'i', 'm', 'e' };
        var correctMessage = string
            .Join(
                "",
                message
                    .ToCharArray()
                    .Where(ch => symbols.Contains(ch))
            );
        var sb = new StringBuilder();
        if (correctMessage.Length == 0)
        {
            return false;
        }

        sb.Append(correctMessage[0]);
        for (var i = 1; i < correctMessage.Length; i++)
        {
            var ch = correctMessage[i];
            if (ch != correctMessage[i - 1])
            {
                sb.Append(ch);
            }
        }

        var result = sb.ToString();
        return result.Contains("anime");
    }
}