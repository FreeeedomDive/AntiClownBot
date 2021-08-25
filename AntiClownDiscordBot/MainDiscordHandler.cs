using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBot.Commands;
using AntiClownBot.Events;
using AntiClownBot.Models.Shop;
using DSharpPlus;
using DSharpPlus.Entities;
using EventHandler = AntiClownBot.Events.EventHandler;
using AntiClownBot.SpecialChannels;
using DSharpPlus.VoiceNext;
using NLog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AntiClownBot
{
    public class MainDiscordHandler
    {
        private static readonly Logger Logger = NLogWrapper.GetDefaultLogger();

        private DiscordClient _discord;
        private readonly Configuration _config;
        private CommandsManager _commandsManager;
        private SpecialChannelsManager _specialChannelsManager;

        public MainDiscordHandler()
        {
            _config = Configuration.GetConfiguration();
            _config.CheckCurrentDay();

            MainTask().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task MainTask()
        {
            _discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "NzYwODc5NjI5NTA5ODUzMjI0.X3SeYA.JLxxQ2gUiFcF9MZyYegkhaDUhqE",
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            });
            _commandsManager = new CommandsManager(_discord, _config);
            _specialChannelsManager = new SpecialChannelsManager(_discord, _config);
            BackendHandler.BackendMessagesLongPolling();
            Utility.Client = _discord;
            _discord.UseVoiceNext();
            Voice.VoiceExtension = _discord.GetVoiceNext();

            _discord.VoiceStateUpdated += (_, e) =>
            {
                if (e.User.IsBot) return Task.CompletedTask;
                if (e.Channel == null) return Task.CompletedTask;
                if (e.Channel.Users.Count() != 1) return Task.CompletedTask;
                
                AddLog("someone is alone");
                new Thread(async () =>
                {
                    await Task.Delay(5 * 60 * 1000);
                    var chlen = await _discord.Guilds[277096298761551872].GetMemberAsync(e.User.Id);
                    if (chlen.VoiceState.Channel != null && chlen.VoiceState.Channel.Users.ToList().Count == 1)
                    {
                        await chlen.ModifyAsync(async model =>
                        {
                            var result = await _discord.GetChannelAsync(689120451984621605);
                            model.VoiceChannel = result;
                        });
                    }
                }).Start();

                return Task.CompletedTask;
            };

            _discord.GuildEmojisUpdated += async (_, e) =>
            {
                if (e.EmojisAfter.Count > e.EmojisBefore.Count)
                {
                    var messageBuilder =
                        new StringBuilder($"{Utility.Emoji(":pepeLaugh:")} {Utility.Emoji(":point_right:")}");
                    foreach (var (key, emoji) in e.EmojisAfter)
                    {
                        if (!e.EmojisBefore.ContainsKey(key))
                        {
                            messageBuilder.Append($" {emoji}");
                        }
                    }

                    await Utility.Client
                        .Guilds[277096298761551872]
                        .GetChannel(838477706643374090)
                        .SendMessageAsync(messageBuilder.ToString());
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

                    messageBuilder.Append($"удалили {Utility.Emoji(":BibleThump:")}");

                    await Utility.Client
                        .Guilds[277096298761551872]
                        .GetChannel(838477706643374090)
                        .SendMessageAsync(messageBuilder.ToString());
                }
            };

            _discord.MessageCreated += async (_, e) =>
            {
                if (e.Author.IsBot) return;

                var message = e.Message.Content;

                AddLog($"{e.Author.Username}: {message}");

                if (message.StartsWith("!"))
                {
                    var commandName = message.Split('\n')[0].Split(' ').First().ToLower();
                    _commandsManager.ExecuteCommand(commandName, e);
                    return;
                }

                if (_specialChannelsManager.AllChannels.Contains(e.Channel.Id))
                {
                    _specialChannelsManager.ParseMessage(e);
                    return;
                }

                CheckEmojiInMessage(message);

                var pidor = Randomizer.GetRandomNumberBetween(0, 25);
                message = message.ToLower();

                if (message.Contains("<@&747723060441776238>") || message.Contains("<@&785512028931489802>"))
                {
                    var emotes = e.Guild.Emojis.Values.ToList();
                    var index = Randomizer.GetRandomNumberBetween(0, emotes.Count);
                    await e.Message.CreateReactionAsync(emotes[index]);
                }

                if (message.Length > 0 && message[^1] == '?')
                {
                    if (message.Contains("когда"))
                    {
                        await e.Message.RespondAsync("Завтра в 3");
                        return;
                    }

                    if ((message.Contains("бот, ты") || message.Contains("бот ты")) &&
                        Randomizer.GetRandomNumberBetween(0, 3) == 0)
                    {
                        await e.Message.RespondAsync("А может ты?");
                        return;
                    }

                    if (Randomizer.FlipACoin())
                    {
                        await e.Message.RespondAsync($"{Utility.StringEmoji(":YEP:")}");
                    }
                    else
                    {
                        await e.Message.RespondAsync($"{Utility.StringEmoji(":NOPE:")}");
                    }

                    return;
                }

                if (message.StartsWith("fuckbot play"))
                {
                    if (pidor != 13) return;
                    if (Randomizer.FlipACoin())
                    {
                        await e.Message.RespondAsync(
                            "https://tenor.com/view/cat-cat-jam-nod-pet-kitty-gif-17932554");
                    }
                    else
                    {
                        await e.Message.RespondAsync(
                            "https://media.discordapp.net/attachments/654674644590264340/768151111252967424/3x.gif");
                    }

                    _config.ChangeBalance(e.Author.Id, 50, "Одобрительная императором музыка");

                    return;
                }

                if (message.Contains("<@!760879629509853224>"))
                {
                    ReactToAppeal(e.Channel);
                    _config.ChangeBalance(e.Author.Id, -25, "Пинг императора");
                    return;
                }

                if (message.Contains("бот") || message.Contains("бинар") || message.Contains("двоич") ||
                    message.Contains("ошибка"))
                {
                    if (pidor < 6)
                    {
                        ReactToAppeal(e.Channel);
                        _config.ChangeBalance(e.Author.Id, -30, "Оскорбление императора");
                        return;
                    }
                }

                if (message.StartsWith("http"))
                {
                    if (pidor < 2)
                    {
                        await e.Message.RespondAsync("Что за говно ты высрал?");
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
                        var didSomeoneSayCock = Utility.Emoji(":DIDSOMEONESAYCOCK:");
                        await e.Message.CreateReactionAsync(didSomeoneSayCock);
                        var yep = Utility.StringEmoji(":YEP:");
                        await e.Message.RespondAsync($"{yep} COCK");
                    }

                    if (booba)
                    {
                        if (Randomizer.FlipACoin())
                        {
                            await e.Message.RespondAsync(
                                "https://tenor.com/view/booba-boobs-coom-hecute-pepe-gif-19186230");
                        }
                        else
                        {
                            await e.Message.RespondAsync(
                                "https://tenor.com/view/booba-pepe-meme-4chan-huypenis-gif-18858228");
                        }
                    }

                    if (anime)
                    {
                        await e.Message.CreateReactionAsync(Utility.Emoji(":AYAYABASS:"));
                    }
                }
            };

            _discord.MessageReactionAdded += async (_, e) =>
            {
                if (e.User.IsBot) return;

                var emoji = e.Emoji;
                var emojiName = emoji.Name;

                if (_config.Shops.TryGetValue(e.User.Id, out var shop) && e.Message.Id == shop.Message.Id)
                {
                    switch (emojiName)
                    {
                        case "1️⃣":
                            shop.HandleItemInSlot(1);
                            await e.Message.DeleteReactionAsync(emoji, e.User);
                            break;
                        case "2️⃣":
                            shop.HandleItemInSlot(2);
                            await e.Message.DeleteReactionAsync(emoji, e.User);
                            break;
                        case "3️⃣":
                            shop.HandleItemInSlot(3);
                            await e.Message.DeleteReactionAsync(emoji, e.User);
                            break;
                        case "4️⃣":
                            shop.HandleItemInSlot(4);
                            await e.Message.DeleteReactionAsync(emoji, e.User);
                            break;
                        case "5️⃣":
                            shop.HandleItemInSlot(5);
                            await e.Message.DeleteReactionAsync(emoji, e.User);
                            break;
                        case "COGGERS":
                            shop.ReRoll();
                            await e.Message.DeleteReactionAsync(emoji, e.User);
                            break;
                        case "pepeSearching":
                            shop.CurrentInstrument = Instrument.Revealing;
                            break;
                    }
                }
                
                switch (emojiName)
                {
                    case "YEP":
                    {
                        var isPartyExists = _config.OpenParties.TryGetValue(e.Message.Id, out var party);
                        if (isPartyExists)
                        {
                            party.JoinParty(e.User.Id);
                        }

                        break;
                    }
                    case "MEGALUL":
                    {
                        var isPartyExists = _config.OpenParties.TryGetValue(e.Message.Id, out var party);
                        if (isPartyExists)
                        {
                            party.Destroy(e.User.Id);
                        }

                        break;
                    }
                    case "NOTED" when _config.CurrentLottery != null &&
                                      _config.CurrentLottery.LotteryMessageId == e.Message.Id &&
                                      _config.CurrentLottery.IsJoinable &&
                                      !_config.CurrentLottery.Participants.Contains(e.User.Id):
                        _config.CurrentLottery.Join(e.User.Id);
                        break;
                    case "monkaSTEER" when _config.CurrentRace is not null &&
                                           _config.CurrentRace.JoinableMessageId == e.Message.Id:
                        _config.CurrentRace.JoinRace(e.User.Id);
                        break;
                }
                
                if (_config.CurrentGuessNumberGame != null &&
                    _config.CurrentGuessNumberGame.GuessNumberGameMessageMessageId == e.Message.Id)
                {
                    switch (emojiName)
                    {
                        case "1️⃣":
                            _config.CurrentGuessNumberGame.Join(e.User.Id, 1);
                            break;
                        case "2️⃣":
                            _config.CurrentGuessNumberGame.Join(e.User.Id, 2);
                            break;
                        case "3️⃣":
                            _config.CurrentGuessNumberGame.Join(e.User.Id, 3);
                            break;
                        case "4️⃣":
                            _config.CurrentGuessNumberGame.Join(e.User.Id, 4);
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
                    AddLog($"Exception in user: {ex.Message}");
                }

                AddLog($"EMOTE ADDED - {username}: {emojiName}");

                _config.AddEmoji(emojiName);

                if ((emojiName is "peepoClown" or "roflanClownbalo" or "clownChamp") && e.User.Id == 423498706336088085)
                {
                    AddLog("Removed clown");
                    await e.Message.DeleteReactionAsync(emoji, e.User);
                }
            };

            _discord.MessageDeleted += (_, e) =>
            {
                _config.DeleteObserverIfExists(e.Message);
                return Task.CompletedTask;
            };

            _discord.MessageReactionRemoved += async (_, e) =>
            {
                var emoji = e.Emoji;
                var emojiName = emoji.Name;

                if (_config.Shops.TryGetValue(e.User.Id, out var shop) && e.Message.Id == shop.Message.Id)
                {
                    if (emojiName == "pepeSearching")
                    {
                        shop.CurrentInstrument = Instrument.Buying;
                    }
                }
                
                switch (emojiName)
                {
                    case "YEP":
                    {
                        var isPartyExists = _config.OpenParties.TryGetValue(e.Message.Id, out var party);
                        if (isPartyExists)
                        {
                            party.LeaveParty(e.User.Id);
                        }

                        break;
                    }
                    case "PogOff" when e.Message.Id == 838796516696391720:
                    {
                        var member = await e.Guild.GetMemberAsync(e.User.Id);
                        var role = e.Guild.GetRole(838794615334633502);
                        await member.RevokeRoleAsync(role);
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
                    AddLog($"Exception in user: {ex.Message}");
                }

                AddLog($"EMOTE REMOVED - {username}: {emojiName}");
                _config.RemoveEmoji(emojiName);
            };

            // for changelogs
            // new Thread(async () =>
            // {
            //     await Task.Delay(10000);
            //     ChangeLog();
            // }).Start();

            new EventHandler(_discord).Start();
            new DailyEventHandler().Start();

            await _discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private async void ReactToAppeal(DiscordChannel channel)
        {
            var pool = new[]
            {
                "Ты понимаешь, что общаешься с бинарником?",
                $"{Utility.StringEmoji(":PogOff:")}",
                "*икает*",
                $"{Utility.StringEmoji(":gachiBASS:")} - мое ебало, когда у кого-то сгорело из-за меня",
                $"{Utility.StringEmoji(":monkaW:")}",
            };
            await channel.SendMessageAsync(pool[Randomizer.GetRandomNumberBetween(0, pool.Length)]);
        }

        private void CheckEmojiInMessage(string message)
        {
            const string pattern = "<a?:(\\S+?):\\d+>";
            var regex = new Regex(pattern);
            var matches = regex.Matches(message);
            foreach (Match match in matches)
            {
                var emote = match.Groups[1].Value;
                _config.AddEmoji(emote);
            }
        }

        private static bool IsCockInMessage(string message)
        {
            var symbols = new[] {'c', 'o', 'k'};
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
            var symbols = new[] {'b', 'o'};
            var correctMessage = string.Join("",
                message
                    .ToCharArray()
                    .Where(ch => symbols.Contains(ch)));
            Console.WriteLine(correctMessage);
            return correctMessage.Contains("boob");
        }

        private static bool IsAnimeInMessage(string message)
        {
            var symbols = new[] {'a', 'n', 'i', 'm', 'e'};
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

        private static void AddLog(string content)
        {
            Logger.Info(content);
        }
    }
}