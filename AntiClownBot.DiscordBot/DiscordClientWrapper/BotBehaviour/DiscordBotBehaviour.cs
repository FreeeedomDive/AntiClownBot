﻿using System.Text;
using System.Text.RegularExpressions;
using AntiClownDiscordBotVersion2.Commands;
using AntiClownDiscordBotVersion2.EventServices;
using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Models.Inventory;
using AntiClownDiscordBotVersion2.Models.Shop;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Statistics.Emotes;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.BotBehaviour;

public class DiscordBotBehaviour : IDiscordBotBehaviour
{
    public DiscordBotBehaviour(
        DiscordClient discordClient,
        IDiscordClientWrapper discordClientWrapper,
        IUserBalanceService userBalanceService,
        ICommandsService commandsService,
        IGuildSettingsService guildSettingsService,
        IEmoteStatsService emoteStatsService,
        IShopService shopService,
        IUserInventoryService userInventoryService,
        IPartyService partyService,
        ILotteryService lotteryService,
        IRaceService raceService,
        IGuessNumberService guessNumberService,
        IRandomizer randomizer,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        this.discordClientWrapper = discordClientWrapper;
        this.userBalanceService = userBalanceService;
        this.commandsService = commandsService;
        this.guildSettingsService = guildSettingsService;
        this.emoteStatsService = emoteStatsService;
        this.shopService = shopService;
        this.userInventoryService = userInventoryService;
        this.partyService = partyService;
        this.lotteryService = lotteryService;
        this.raceService = raceService;
        this.guessNumberService = guessNumberService;
        this.randomizer = randomizer;
        this.logger = logger;
    }
    
    public void Configure()
    {
        discordClient.GuildEmojisUpdated += GuildEmojisUpdated;
        discordClient.MessageCreated += MessageCreated;
        discordClient.MessageReactionAdded += MessageReactionAdded;
        discordClient.MessageDeleted += MessageDeleted;
        discordClient.MessageReactionRemoved += MessageReactionRemoved;
    }

    private async Task GuildEmojisUpdated(DiscordClient _, GuildEmojisUpdateEventArgs e)
    {
        if (e.EmojisAfter.Count > e.EmojisBefore.Count)
        {
            var messageBuilder = new StringBuilder($"{await discordClientWrapper.Emotes.FindEmoteAsync("pepeLaugh")} " +
                                                   $"{await discordClientWrapper.Emotes.FindEmoteAsync("point_right")}");
            foreach (var (key, emoji) in e.EmojisAfter)
            {
                if (!e.EmojisBefore.ContainsKey(key))
                {
                    messageBuilder.Append($" {emoji}");
                }
            }

            await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageBuilder.ToString());
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

            messageBuilder.Append($"удалили {await discordClientWrapper.Emotes.FindEmoteAsync("BibleThump")}");
            await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageBuilder.ToString());
        }
    }

    private async Task MessageCreated(DiscordClient _, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot) return;

        var guildSettings = guildSettingsService.GetGuildSettings();

        var message = e.Message.Content;

        logger.Info($"{e.Author.Username}: {message}");

        if (message.StartsWith(guildSettings.CommandsPrefix))
        {
            var commandName = message.Split('\n')[0].Split(' ').First().ToLower();
            await commandsService.ExecuteCommand(commandName, e);
            return;
        }

        /* TODO: temp disabled
        if (_specialChannelsManager.AllChannels.Contains(e.Channel.Id))
        {
            _specialChannelsManager.ParseMessage(e);
            return;
        }*/

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

            if ((message.Contains("бот, ты") || message.Contains("бот ты")) && randomizer.GetRandomNumberBetween(0, 3) == 0)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "А может ты?");
                return;
            }

            if (randomizer.FlipACoin())
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message,$"{await discordClientWrapper.Emotes.FindEmoteAsync("YEP")}");
            }
            else
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message,$"{await discordClientWrapper.Emotes.FindEmoteAsync("NOPE")}");
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
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(e.Author.Id, -30, "Оскорбление императора");
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
                var didSomeoneSayCock = await discordClientWrapper.Emotes.FindEmoteAsync("DIDSOMEONESAYCOCK");
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(e.Message, didSomeoneSayCock);
                var yep = await discordClientWrapper.Emotes.FindEmoteAsync("YEP");
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(e.Message, $"{yep} COCK");
            }

            if (booba)
            {
                if (randomizer.FlipACoin())
                {
                    await discordClientWrapper.Messages.RespondAsync(e.Message, "https://tenor.com/view/booba-boobs-coom-hecute-pepe-gif-19186230");
                }
                else
                {
                    await discordClientWrapper.Messages.RespondAsync(e.Message, "https://tenor.com/view/booba-pepe-meme-4chan-huypenis-gif-18858228");
                }
            }

            if (anime)
            {
                await discordClientWrapper.Emotes.AddReactionToMessageAsync(e.Message, await discordClientWrapper.Emotes.FindEmoteAsync("AYAYABASS"));
            }
        }
    }

    private async Task MessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
    {
        if (e.User.IsBot) return;

        var emoji = e.Emoji;
        var emojiName = emoji.Name;

        if (shopService.TryRead(e.User.Id, out var shop) && e.Message.Id == shop.Message.Id)
        {
            switch (emojiName)
            {
                case "1️⃣":
                    await shop.HandleItemInSlot(1);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "2️⃣":
                    await shop.HandleItemInSlot(2);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "3️⃣":
                    await shop.HandleItemInSlot(3);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "4️⃣":
                    await shop.HandleItemInSlot(4);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "5️⃣":
                    await shop.HandleItemInSlot(5);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "COGGERS":
                    await shop.ReRoll();
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "pepeSearching":
                    shop.CurrentShopTool = ShopTool.Revealing;
                    break;
            }
        }

        if (userInventoryService.TryRead(e.User.Id, out var inventory) && e.Message.Id == inventory.Message.Id)
        {
            switch (emojiName)
            {
                case "⬅️":
                case "arrow_left":
                    await inventory.SwitchLeftPage();
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "➡️":
                case "arrow_right":
                    await inventory.SwitchRightPage();
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "1️⃣":
                    await inventory.HandleItemInSlot(1);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "2️⃣":
                    await inventory.HandleItemInSlot(2);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "3️⃣":
                    await inventory.HandleItemInSlot(3);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "4️⃣":
                    await inventory.HandleItemInSlot(4);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "5️⃣":
                    await inventory.HandleItemInSlot(5);
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "🔁":
                case "repeat":
                    await inventory.EnableChangingStatus();
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
                case "❌":
                case "x":
                    await inventory.EnableSelling();
                    await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
                    break;
            }
        }

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
            logger.Error($"Exception in username", ex);
        }

        logger.Info($"EMOTE ADDED - {username}: {emojiName}");

        emoteStatsService.AddStats(emojiName);

        if ((emojiName is "peepoClown" or "clown" or "clown_face") && (e.User.Id is 423498706336088085 or 369476500820459522))
        {
            logger.Info("Removed clown");
            await discordClientWrapper.Emotes.RemoveReactionFromMessageAsync(e.Message, emoji, e.User);
        }
    }

    private async Task MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
    {
        var emoji = e.Emoji;
        var emojiName = emoji.Name;

        if (shopService.TryRead(e.User.Id, out var shop) && e.Message.Id == shop.Message.Id)
        {
            if (emojiName == "pepeSearching")
            {
                shop.CurrentShopTool = ShopTool.Buying;
            }
        }
                
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
            logger.Error($"Exception in username", ex);
        }

        logger.Info($"EMOTE REMOVED - {username}: {emojiName}"); 
        emoteStatsService.RemoveStats(emojiName);
    }

    private Task MessageDeleted(DiscordClient sender, MessageDeleteEventArgs e)
    {
        partyService.DeleteObserverIfExists(e.Message);

        return Task.CompletedTask;
    }

    private async Task ReactToAppeal(DiscordMessage message)
    {
        var pool = new[]
        {
            "Ты понимаешь, что общаешься с бинарником?",
            $"{await discordClientWrapper.Emotes.FindEmoteAsync("PogOff")}",
            "*икает*",
            $"{await discordClientWrapper.Emotes.FindEmoteAsync("gachiBASS")}",
            $"{await discordClientWrapper.Emotes.FindEmoteAsync("monkaW")}",
        };
        await discordClientWrapper.Messages.RespondAsync(message, pool[randomizer.GetRandomNumberBetween(0, pool.Length)]);
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

    private readonly DiscordClient discordClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IUserBalanceService userBalanceService;
    private readonly ICommandsService commandsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IEmoteStatsService emoteStatsService;
    private readonly IShopService shopService;
    private readonly IUserInventoryService userInventoryService;
    private readonly IPartyService partyService;
    private readonly ILotteryService lotteryService;
    private readonly IRaceService raceService;
    private readonly IGuessNumberService guessNumberService;
    private readonly IRandomizer randomizer;
    private readonly ILogger logger;
}