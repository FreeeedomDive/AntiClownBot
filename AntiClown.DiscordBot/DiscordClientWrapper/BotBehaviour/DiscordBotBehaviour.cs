using System.Text;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.SlashCommands.SocialRating;
using AntiClown.Tools.Utility.Extensions;
using AntiClown.Tools.Utility.Random;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;

public class DiscordBotBehaviour : IDiscordBotBehaviour
{
    public DiscordBotBehaviour(
        IServiceProvider serviceProvider,
        DiscordClient discordClient,
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IOptions<DiscordOptions> discordOptions,
        IOptions<Settings> settings
        /*
        IShopService shopService,
        IUserInventoryService userInventoryService,
        IPartyService partyService,
        ILotteryService lotteryService,
        IRaceService raceService,
        IGuessNumberService guessNumberService,
        IF1PredictionsService f1PredictionsService,
        ILoggerClient logger
        */
    )
    {
        this.serviceProvider = serviceProvider;
        this.discordClient = discordClient;
        this.discordClientWrapper = discordClientWrapper;
        this.emotesCache = emotesCache;
        this.discordOptions = discordOptions;
        this.settings = settings;
    }

    public async Task ConfigureAsync()
    {
        discordClient.GuildEmojisUpdated += GuildEmojisUpdated;
        discordClient.MessageCreated += MessageCreated;
        discordClient.MessageReactionAdded += MessageReactionAdded;
        discordClient.ComponentInteractionCreated += ComponentInteractionCreated;
        await RegisterSlashCommands(discordClient);
    }

    private async Task GuildEmojisUpdated(DiscordClient _, GuildEmojisUpdateEventArgs e)
    {
        if (!settings.Value.IsEmoteNotificationEnabled)
        {
            return;
        }

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
                discordOptions.Value.BotChannelId,
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
                discordOptions.Value.BotChannelId,
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

        // удаляем все сообщения из чата с пати, чтобы люди отвечали в треды
        if (e.Channel.Id == discordOptions.Value.PartyChannelId)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                               //.WithTitle("Модерация чата пати")
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

        if (settings.Value.XddAnswersEnabled && Randomizer.GetRandomNumberBetween(0, 25) == 0)
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

        if (emojiName is "peepoClown" or "clown" or "clown_face" &&
            e.User.Id is 423498706336088085 or 369476500820459522)
        {
            // await logger.InfoAsync("Removed clown");
            await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
        }

        /*
        TODO: handlers for events, based on interaction ID
        switch (emojiName)
        {
            case "YEP":
            {
                var isPartyExists = partyService.PartiesInfo.OpenParties.TryGetValue(e.Message.Id, out var party);
                if (isPartyExists && party != null)
                {
                    await party.JoinParty(e.User.Id);
                }

                break;
            }
            case "MEGALUL":
            {
                var isPartyExists = partyService.PartiesInfo.OpenParties.TryGetValue(e.Message.Id, out var party);
                if (isPartyExists && party != null)
                {
                    await party.Destroy(e.User.Id);
                }

                break;
            }
            case "NOTED" when lotteryService.Lottery is not null &&
                              lotteryService.Lottery.MessageId == e.Message.Id &&
                              lotteryService.Lottery.IsJoinable &&
                              !lotteryService.Lottery.Participants.Contains(e.User.Id):
                await lotteryService.Lottery.Join(e.User.Id);
                break;
            case "monkaSTEER" when raceService.Race is not null &&
                                   raceService.Race.JoinableMessageId == e.Message.Id:
                raceService.Race.JoinRace(e.User.Id);
                break;
        }

        if (guessNumberService.CurrentGame is not null &&
            guessNumberService.CurrentGame.GuessNumberGameMessageMessageId == e.Message.Id)
        {
            switch (emojiName)
            {
                case "1️⃣":
                    guessNumberService.CurrentGame.Join(e.User.Id, 1);
                    break;
                case "2️⃣":
                    guessNumberService.CurrentGame.Join(e.User.Id, 2);
                    break;
                case "3️⃣":
                    guessNumberService.CurrentGame.Join(e.User.Id, 3);
                    break;
                case "4️⃣":
                    guessNumberService.CurrentGame.Join(e.User.Id, 4);
                    break;
                default:
                    return;
            }
        }
        */
    }

    /*
    TODO: same with method above
    private async Task MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
    {
        var emoji = e.Emoji;
        var emojiName = emoji.Name;

        switch (emojiName)
        {
            case "YEP":
            {
                var isPartyExists = partyService.PartiesInfo.OpenParties.TryGetValue(e.Message.Id, out var party);
                if (isPartyExists && party != null)
                {
                    await party.LeaveParty(e.User.Id);
                }

                break;
            }
        }
    }
    */

    private async Task ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
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

        await discordClientWrapper.Messages.RespondAsync(
            e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null
        );
        if (e.Id.StartsWith(InteractionsIds.ShopButtons.Prefix))
        {
            await HandleShopInteraction(e);
            return;
        }

        if (e.Id.StartsWith(InteractionsIds.InventoryButtons.Prefix))
        {
            await HandleInventoryInteraction(e);
            return;
        }

        if (e.Id.StartsWith(InteractionsIds.F1PredictionsButtons.StartRaceResultInputButton))
        {
            await HandleRaceResultInput(e, true);
            return;
        }

        if (e.Id.StartsWith(InteractionsIds.F1PredictionsButtons.DriversSelectDropdown))
        {
            await HandleRaceResultInput(e);
        }
    }

    private async Task HandleShopInteraction(ComponentInteractionCreateEventArgs e)
    {
        /*if (!shopService.TryRead(e.User.Id, out var shop))
        {
            return;
        }

        var builder = new DiscordWebhookBuilder();
        switch (e.Id)
        {
            case Interactions.Buttons.ShopButtonItem1:
                await shop.HandleItemInSlot(1, e.Interaction);
                break;
            case Interactions.Buttons.ShopButtonItem2:
                await shop.HandleItemInSlot(2, e.Interaction);
                break;
            case Interactions.Buttons.ShopButtonItem3:
                await shop.HandleItemInSlot(3, e.Interaction);
                break;
            case Interactions.Buttons.ShopButtonItem4:
                await shop.HandleItemInSlot(4, e.Interaction);
                break;
            case Interactions.Buttons.ShopButtonItem5:
                await shop.HandleItemInSlot(5, e.Interaction);
                break;
            case Interactions.Buttons.ShopButtonReroll:
                await shop.ReRoll(e.Interaction);
                break;
            case Interactions.Buttons.ShopButtonChangeTool:
                shop.CurrentShopTool =
                    shop.CurrentShopTool == ShopTool.Revealing ? ShopTool.Buying : ShopTool.Revealing;
                break;
        }

        await discordClientWrapper.Messages.EditOriginalResponseAsync(
            e.Interaction,
            await builder.AddEmbed(await shop.GetNewShopEmbed()).SetShopButtons(emotesProvider)
        );*/
    }

    private async Task HandleInventoryInteraction(ComponentInteractionCreateEventArgs e)
    {
        /*if (!userInventoryService.TryRead(e.User.Id, out var inventory))
        {
            return;
        }

        var builder = new DiscordWebhookBuilder();
        switch (e.Id)
        {
            case Interactions.Buttons.InventoryButton1:
                builder.Content = await inventory.HandleItemInSlot(1);
                break;
            case Interactions.Buttons.InventoryButton2:
                builder.Content = await inventory.HandleItemInSlot(2);
                break;
            case Interactions.Buttons.InventoryButton3:
                builder.Content = await inventory.HandleItemInSlot(3);
                break;
            case Interactions.Buttons.InventoryButton4:
                builder.Content = await inventory.HandleItemInSlot(4);
                break;
            case Interactions.Buttons.InventoryButton5:
                builder.Content = await inventory.HandleItemInSlot(5);
                break;
            case Interactions.Buttons.InventoryButtonLeft:
                await inventory.SwitchLeftPage();
                break;
            case Interactions.Buttons.InventoryButtonRight:
                await inventory.SwitchRightPage();
                break;
            case Interactions.Buttons.InventoryButtonChangeActiveStatus:
                await inventory.EnableChangingStatus();
                break;
            case Interactions.Buttons.InventoryButtonSell:
                await inventory.EnableSelling();
                break;
        }

        await discordClientWrapper.Messages.EditOriginalResponseAsync(
            e.Interaction,
            await builder.AddEmbed(inventory.UpdateEmbedForCurrentPage()).SetInventoryButtons(emotesProvider)
        );*/
    }

    private async Task HandleRaceResultInput(ComponentInteractionCreateEventArgs e, bool start = false)
    {
        /*
        TODO: move to EntertainmentApi
        if (!start)
        {
            var driverName = e.Values.First()[InteractionsIds.F1PredictionsButtons.DriversSelectDropdownItemPrefix.Length..];
            var driver = Enum.TryParse(typeof(F1Driver), driverName, out var result)
                ? (F1Driver)result
                : throw new ArgumentException($"Unexpected driver {driverName}");
            f1PredictionsService.AddDriverToResult(driver);
            var drivers = f1PredictionsService.DriversToAddToResult();
            if (drivers.Length == 0)
            {
                var results = f1PredictionsService.MakeTenthPlaceResults();
                if (results.Length == 0)
                {
                    await discordClientWrapper.Messages.EditOriginalResponseAsync(
                        e.Interaction,
                        new DiscordWebhookBuilder().WithContent("Никто не вносил предсказаний")
                    );
                    return;
                }

                var members = (await discordClientWrapper.Guilds.GetGuildAsync()).Members;
                var resultsStrings =
                    results.Select(tuple => $"{members[tuple.userId].ServerOrUserName()}: {tuple.tenthPlacePoints}");
                await discordClientWrapper.Messages.EditOriginalResponseAsync(
                    e.Interaction,
                    new DiscordWebhookBuilder().WithContent(string.Join("\n", resultsStrings))
                );
                return;
            }
        }

        var updatedDrivers = f1PredictionsService.DriversToAddToResult();
        var options = updatedDrivers.Select(
            x => new DiscordSelectComponentOption(
                x.ToString(),
                $"{Interactions.Dropdowns.DriversSelectDropdownItemPrefix}{x.ToString()}"
            )
        );
        var currentPlaceToEnter = 20 - updatedDrivers.Length + 1;
        var dropdown = new DiscordSelectComponent(
            Interactions.Dropdowns.DriversSelectDropdown,
            $"Гонщик на {currentPlaceToEnter} месте", options
        );
        var builder = new DiscordWebhookBuilder()
                      .WithContent($"Результаты гонки, {currentPlaceToEnter} место")
                      .AddComponents(dropdown);

        await discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction, builder);*/
    }

    private async Task RegisterSlashCommands(DiscordClient client)
    {
        // await logger.InfoAsync("Register slash commands");
        var guildId = discordOptions.Value.GuildId;
        var slash = client.UseSlashCommands(
            new SlashCommandsConfiguration
            {
                Services = serviceProvider,
            }
        );
        // slash.RegisterCommands<PartyCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<InventoryCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<LohotronCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<RatingCommandModule>(guildId);
        // slash.RegisterCommands<RolesCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<LotteryCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<ChangeNicknameCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<DailyResetCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<RaceCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<TributeCommandModule>(guildId);
        slash.RegisterCommands<WhenCommandModule>(guildId);
        // slash.RegisterCommands<RollCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<IpCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<SelectCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<EmojiStatsCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<F1CommandModule>(guildSettings.GuildId);
        // // admin commands
        // slash.RegisterCommands<UserSocialRatingEditorCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<CreateMessageCommandModule>(guildSettings.GuildId);
        // slash.RegisterCommands<F1AdminCommandModule>(guildSettings.GuildId);
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

    private readonly DiscordClient discordClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly IServiceProvider serviceProvider;
    private readonly IOptions<Settings> settings;
}