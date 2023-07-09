using System.Text;
using System.Text.RegularExpressions;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.EventServices;
using AntiClownDiscordBotVersion2.Models.F1;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.Models.Inventory;
using AntiClownDiscordBotVersion2.Models.Shop;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.SlashCommands.Dev;
using AntiClownDiscordBotVersion2.SlashCommands.Gaming;
using AntiClownDiscordBotVersion2.SlashCommands.Inventory;
using AntiClownDiscordBotVersion2.SlashCommands.Lohotron;
using AntiClownDiscordBotVersion2.SlashCommands.Other;
using AntiClownDiscordBotVersion2.SlashCommands.Other.F1Predictions;
using AntiClownDiscordBotVersion2.SlashCommands.Other.Ip;
using AntiClownDiscordBotVersion2.SlashCommands.Random;
using AntiClownDiscordBotVersion2.SlashCommands.Roles;
using AntiClownDiscordBotVersion2.SlashCommands.SocialRating;
using AntiClownDiscordBotVersion2.Statistics.Emotes;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.BotBehaviour;

public class DiscordBotBehaviour : IDiscordBotBehaviour
{
    public DiscordBotBehaviour(
        IServiceProvider serviceProvider,
        DiscordClient discordClient,
        IDiscordClientWrapper discordClientWrapper,
        IUserBalanceService userBalanceService,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService,
        IEmoteStatsService emoteStatsService,
        IShopService shopService,
        IUserInventoryService userInventoryService,
        IPartyService partyService,
        ILotteryService lotteryService,
        IRaceService raceService,
        IGuessNumberService guessNumberService,
        IRandomizer randomizer,
        IF1PredictionsService f1PredictionsService,
        IEmotesProvider emotesProvider,
        ILoggerClient logger
    )
    {
        this.serviceProvider = serviceProvider;
        this.discordClient = discordClient;
        this.discordClientWrapper = discordClientWrapper;
        this.userBalanceService = userBalanceService;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
        this.emoteStatsService = emoteStatsService;
        this.shopService = shopService;
        this.userInventoryService = userInventoryService;
        this.partyService = partyService;
        this.lotteryService = lotteryService;
        this.raceService = raceService;
        this.guessNumberService = guessNumberService;
        this.randomizer = randomizer;
        this.f1PredictionsService = f1PredictionsService;
        this.emotesProvider = emotesProvider;
        this.logger = logger;
    }

    public async Task ConfigureAsync()
    {
        discordClient.GuildEmojisUpdated += GuildEmojisUpdated;
        discordClient.MessageCreated += MessageCreated;
        discordClient.MessageReactionAdded += MessageReactionAdded;
        discordClient.MessageDeleted += MessageDeleted;
        discordClient.MessageReactionRemoved += MessageReactionRemoved;
        discordClient.ComponentInteractionCreated += ComponentInteractionCreated;
        await RegisterSlashCommands(discordClient);
    }

    private async Task GuildEmojisUpdated(DiscordClient _, GuildEmojisUpdateEventArgs e)
    {
        var appSettings = appSettingsService.GetSettings();
        if (!appSettings.IsEmoteNotificationEnabled)
        {
            return;
        }

        if (e.EmojisAfter.Count > e.EmojisBefore.Count)
        {
            var messageBuilder = new StringBuilder($"{await emotesProvider.GetEmoteAsTextAsync("pepeLaugh")} " +
                                                   $"{await emotesProvider.GetEmoteAsTextAsync("point_right")}");
            foreach (var (key, emoji) in e.EmojisAfter)
            {
                if (!e.EmojisBefore.ContainsKey(key))
                {
                    messageBuilder.Append($" {emoji}");
                }
            }

            await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId,
                messageBuilder.ToString());
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

            messageBuilder.Append($"удалили {await emotesProvider.GetEmoteAsTextAsync("BibleThump")}");
            await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId,
                messageBuilder.ToString());
        }
    }

    private async Task MessageCreated(DiscordClient _, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot) return;

        var guildSettings = guildSettingsService.GetGuildSettings();

        var message = e.Message.Content;

        // удаляем все сообщения из чата с пати, чтобы люди отвечали в треды
        if (e.Channel.Id == guildSettings.PartyChannelId)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                //.WithTitle("Модерация чата пати")
                .WithColor(DiscordColor.DarkRed)
                .AddField("Модерация чата пати",
                    "Если нужно собрать пати, воспользуйся командой !party или слеш-командой /party\n" +
                    "Если нужно ответить по какому-то пати, сделай это в соответствующем треде");
            var messageBuilder = new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build())
                .WithAllowedMentions(Mentions.All)
                .WithContent(e.Author.Mention);
            Task.Run(async () =>
            {
                var response = await discordClientWrapper.Messages.RespondAsync(e.Message, messageBuilder);
                await discordClientWrapper.Messages.DeleteAsync(e.Message);
                await Task.Delay(10000);
                await discordClientWrapper.Messages.DeleteAsync(response);
            });
            return;
        }

        CheckEmojiInMessage(message);

        var pidor = randomizer.GetRandomNumberBetween(0, 25);
        message = message.ToLower();

        if (message.Length > 0 && message[^1] == '?')
        {
            if (message.Contains("когда"))
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "Завтра в 3");
                return;
            }

            if ((message.Contains("бот, ты") || message.Contains("бот ты")) &&
                randomizer.GetRandomNumberBetween(0, 3) == 0)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "А может ты?");
                return;
            }

            if (randomizer.FlipACoin())
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message,
                    $"{await emotesProvider.GetEmoteAsTextAsync("YEP")}");
            }
            else
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message,
                    $"{await emotesProvider.GetEmoteAsTextAsync("NOPE")}");
            }

            return;
        }

        var botId = await discordClientWrapper.Members.GetBotIdAsync();
        if (message.Contains($"<@!{botId}>"))
        {
            await ReactToAppeal(e.Message);
            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(e.Author.Id, -25, "Пинг императора");
            return;
        }

        if (message.Contains("бот") || message.Contains("бинар") || message.Contains("двоич") ||
            message.Contains("ошибка"))
        {
            if (pidor < 6)
            {
                await ReactToAppeal(e.Message);
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(e.Author.Id, -30,
                    "Оскорбление императора");
                return;
            }
        }

        var cocks = IsCockInMessage(message);
        var booba = IsBoobaInMessage(message);
        var anime = IsAnimeInMessage(message);

        if ((cocks || booba || anime) && !(message.Contains("youtu") || message.Contains("spotify")))
        {
            if (cocks)
            {
                var didSomeoneSayCock = await emotesProvider.GetEmoteAsTextAsync("DIDSOMEONESAYCOCK");
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(e.Message, didSomeoneSayCock);
                var yep = await emotesProvider.GetEmoteAsTextAsync("YEP");
                await discordClientWrapper.Messages.RespondAsync(e.Message, $"{yep} COCK");
            }

            if (booba)
            {
                if (randomizer.FlipACoin())
                {
                    await discordClientWrapper.Messages.RespondAsync(e.Message,
                        "https://tenor.com/view/booba-boobs-coom-hecute-pepe-gif-19186230");
                }
                else
                {
                    await discordClientWrapper.Messages.RespondAsync(e.Message,
                        "https://tenor.com/view/booba-pepe-meme-4chan-huypenis-gif-18858228");
                }
            }

            if (anime)
            {
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(e.Message,
                    await emotesProvider.GetEmoteAsTextAsync("AYAYABASS"));
            }
        }

        var settings = appSettingsService.GetSettings();
        if (settings.XddAnswersEnabled && randomizer.GetRandomNumberBetween(0, 25) == 0)
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
            var xdd = await emotesProvider.GetEmoteAsTextAsync(xddPool.SelectRandomItem(randomizer));
            await discordClientWrapper.Messages.RespondAsync(e.Message, xdd);
        }
    }

    private async Task MessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
    {
        if (e.User.IsBot) return;

        var emoji = e.Emoji;
        var emojiName = emoji.Name;

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

        var username = "unknown";
        try
        {
            username = e.User.Username;
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex, "Exception in username");
        }

        await logger.InfoAsync($"EMOTE ADDED - {username}: {emojiName}");

        emoteStatsService.AddStats(emojiName);

        if ((emojiName is "peepoClown" or "clown" or "clown_face") &&
            (e.User.Id is 423498706336088085 or 369476500820459522))
        {
            await logger.InfoAsync("Removed clown");
            await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
        }
    }

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

        var username = "unknown";
        try
        {
            username = e.User.Username;
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex, "Exception in username");
        }

        await logger.InfoAsync($"EMOTE REMOVED - {username}: {emojiName}");
        emoteStatsService.RemoveStats(emojiName);
    }

    private async Task ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var interactionAuthor = e.Message.Interaction?.User;
        var responseBuilder = new DiscordInteractionResponseBuilder();
        if (interactionAuthor == null || e.User.Id != interactionAuthor.Id)
        {
            var member = await discordClientWrapper.Members.GetAsync(e.User.Id);
            await discordClientWrapper.Messages.RespondAsync(e.Interaction,
                InteractionResponseType.ChannelMessageWithSource,
                responseBuilder.WithContent($"{member.Mention} " +
                                            $"НЕ НАДО ЮЗАТЬ ЧУЖИЕ КНОПКИ " +
                                            $"{await emotesProvider.GetEmoteAsTextAsync("Madge")}"));
        }

        await discordClientWrapper.Messages.RespondAsync(e.Interaction, InteractionResponseType.DeferredMessageUpdate,
            null);
        if (e.Id.StartsWith(Interactions.Buttons.ShopButtonsPrefix))
        {
            await HandleShopInteraction(e);
            return;
        }

        if (e.Id.StartsWith(Interactions.Buttons.InventoryButtonsPrefix))
        {
            await HandleInventoryInteraction(e);
            return;
        }

        if (e.Id.StartsWith(Interactions.Buttons.StartRaceResultInputButton))
        {
            await HandleRaceResultInput(e, true);
            return;
        }

        if (e.Id.StartsWith(Interactions.Dropdowns.DriversSelectDropdown))
        {
            await HandleRaceResultInput(e);
        }
    }

    private async Task HandleShopInteraction(ComponentInteractionCreateEventArgs e)
    {
        if (!shopService.TryRead(e.User.Id, out var shop))
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

        await discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction,
            await builder.AddEmbed(await shop.GetNewShopEmbed()).SetShopButtons(emotesProvider));
    }

    private async Task HandleInventoryInteraction(ComponentInteractionCreateEventArgs e)
    {
        if (!userInventoryService.TryRead(e.User.Id, out var inventory))
            return;

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

        await discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction,
            await builder.AddEmbed(inventory.UpdateEmbedForCurrentPage()).SetInventoryButtons(emotesProvider));
    }

    private async Task HandleRaceResultInput(ComponentInteractionCreateEventArgs e, bool start = false)
    {
        if (!start)
        {
            var driverName = e.Values.First()[Interactions.Dropdowns.DriversSelectDropdownItemPrefix.Length..];
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
                    await discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction,
                        new DiscordWebhookBuilder().WithContent("Никто не вносил предсказаний"));
                    return;
                }

                var members = (await discordClientWrapper.Guilds.GetGuildAsync()).Members;
                var resultsStrings =
                    results.Select(tuple => $"{members[tuple.userId].ServerOrUserName()}: {tuple.tenthPlacePoints}");
                await discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction,
                    new DiscordWebhookBuilder().WithContent(string.Join("\n", resultsStrings)));
                return;
            }
        }

        var updatedDrivers = f1PredictionsService.DriversToAddToResult();
        var options = updatedDrivers.Select(x => new DiscordSelectComponentOption(
            x.ToString(),
            $"{Interactions.Dropdowns.DriversSelectDropdownItemPrefix}{x.ToString()}"
        ));
        var currentPlaceToEnter = 20 - updatedDrivers.Length + 1;
        var dropdown = new DiscordSelectComponent(Interactions.Dropdowns.DriversSelectDropdown,
            $"Гонщик на {currentPlaceToEnter} месте", options);
        var builder = new DiscordWebhookBuilder()
            .WithContent($"Результаты гонки, {currentPlaceToEnter} место")
            .AddComponents(dropdown);

        await discordClientWrapper.Messages.EditOriginalResponseAsync(e.Interaction, builder);
    }

    private Task MessageDeleted(DiscordClient sender, MessageDeleteEventArgs e)
    {
        partyService.DeleteObserverIfExists(e.Message);

        return Task.CompletedTask;
    }

    private async Task RegisterSlashCommands(DiscordClient client)
    {
        await logger.InfoAsync("Register slash commands");
        var guildSettings = guildSettingsService.GetGuildSettings();
        var slash = client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = serviceProvider
        });
        // TODO: хорошо бы автоматизировать, но он не хочет регать модули через дженерики и GetType
        slash.RegisterCommands<PartyCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<InventoryCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<LohotronCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<RatingCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<RolesCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<LotteryCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<ChangeNicknameCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<DailyResetCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<RaceCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<TributeCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<WhenCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<RollCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<IpCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<SelectCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<EmojiStatsCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<F1CommandModule>(guildSettings.GuildId);
        // admin commands
        slash.RegisterCommands<UserSocialRatingEditorCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<CreateMessageCommandModule>(guildSettings.GuildId);
        slash.RegisterCommands<F1AdminCommandModule>(guildSettings.GuildId);
    }

    private async Task ReactToAppeal(DiscordMessage message)
    {
        var pool = new[]
        {
            "Ты понимаешь, что общаешься с бинарником?",
            "*икает*",
            await emotesProvider.GetEmoteAsTextAsync("PogOff"),
            await emotesProvider.GetEmoteAsTextAsync("gachiBASS"),
            await emotesProvider.GetEmoteAsTextAsync("monkaW"),
            await emotesProvider.GetEmoteAsTextAsync("xdd"),
            await emotesProvider.GetEmoteAsTextAsync("ok"),
            await emotesProvider.GetEmoteAsTextAsync("SHTOOO"),
            await emotesProvider.GetEmoteAsTextAsync("Starege"),
            await emotesProvider.GetEmoteAsTextAsync("peepoPooPoo"),
            await emotesProvider.GetEmoteAsTextAsync("peepoRip"),
            await emotesProvider.GetEmoteAsTextAsync("aRolf"),
        };
        await discordClientWrapper.Messages.RespondAsync(message,
            pool[randomizer.GetRandomNumberBetween(0, pool.Length)]);
    }

    private void CheckEmojiInMessage(string message)
    {
        const string pattern = "<a?:(\\S+?):\\d+>";
        var regex = new Regex(pattern);
        var matches = regex.Matches(message);
        foreach (Match match in matches)
        {
            var emote = match.Groups[1].Value;
            emoteStatsService.AddStats(emote);
        }
    }

    private static bool IsCockInMessage(string message)
    {
        var symbols = new[] { 'c', 'o', 'k' };
        var correctMessage = string
            .Join("",
                message
                    .ToCharArray()
                    .Where(ch => symbols.Contains(ch)));
        var sb = new StringBuilder();
        if (correctMessage.Length == 0) return false;
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
        var correctMessage = string.Join("",
            message
                .ToCharArray()
                .Where(ch => symbols.Contains(ch)));
        return correctMessage.Contains("boob");
    }

    private static bool IsAnimeInMessage(string message)
    {
        var symbols = new[] { 'a', 'n', 'i', 'm', 'e' };
        var correctMessage = string
            .Join("",
                message
                    .ToCharArray()
                    .Where(ch => symbols.Contains(ch)));
        var sb = new StringBuilder();
        if (correctMessage.Length == 0) return false;
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

    private readonly IServiceProvider serviceProvider;
    private readonly DiscordClient discordClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IUserBalanceService userBalanceService;
    private readonly IAppSettingsService appSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IEmoteStatsService emoteStatsService;
    private readonly IShopService shopService;
    private readonly IUserInventoryService userInventoryService;
    private readonly IPartyService partyService;
    private readonly ILotteryService lotteryService;
    private readonly IRaceService raceService;
    private readonly IGuessNumberService guessNumberService;
    private readonly IRandomizer randomizer;
    private readonly IF1PredictionsService f1PredictionsService;
    private readonly IEmotesProvider emotesProvider;
    private readonly ILoggerClient logger;
}