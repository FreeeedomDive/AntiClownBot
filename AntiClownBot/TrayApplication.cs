using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntiClownBot.Commands;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownBot
{
    public class TrayApplication : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private DiscordClient _discord;
        private readonly Configuration _config;
        private CommandsManager _commandsManager;

        private ulong _lastReactionMessageId;
        private ulong _lastReactionUserId;
        private string _lastReactionEmote = "";

        private ulong _lastPidorId;

        public TrayApplication()
        {
            _trayIcon = new NotifyIcon
            {
                Text = @"AntiClown Discord Bot",
                Icon = new Icon("peepoclown.ico"),
                Visible = true,
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Exit", Exit)
                })
            };

            _config = Configuration.GetConfiguration();
            _config.CheckCurrentDay();

            MainTask().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task MainTask()
        {
            _discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "NzYwODc5NjI5NTA5ODUzMjI0.X3SeYA.JLxxQ2gUiFcF9MZyYegkhaDUhqE",
                TokenType = TokenType.Bot
            });

            _commandsManager = new CommandsManager(_discord, _config);
            Utility.Client = _discord;

            _discord.MessageCreated += async (client, e) =>
            {
                var message = e.Message.Content;

                if (e.Author.IsBot) return;

                _config.DecreasePidorRoulette();

                AddLog($"{e.Author.Username}: {message}");

                SocialRatingUser user;
                if (_config.Users.ContainsKey(e.Author.Id))
                    user = _config.Users[e.Author.Id];
                else
                {
                    user = new SocialRatingUser(e.Author.Username);
                    _config.Users.Add(e.Author.Id, user);
                    _config.Save();
                }

                if (message.StartsWith("!"))
                {
                    var commandName = message.Split('\n')[0].Split(' ').First();
                    _commandsManager.ExecuteCommand(commandName, e, user);
                    return;
                }

                var randomMessageRating = Randomizer.GetRandomNumberBetween(-4, 11);
                if (randomMessageRating > 0)
                    Utility.IncreaseRating(_config, user, randomMessageRating, e);
                else if (randomMessageRating < 0)
                    Utility.DecreaseRating(_config, user, -randomMessageRating, e);

                CheckStats(message);

                var pidor = Randomizer.GetRandomNumberBetween(0, 25);
                message = message.ToLower();

                if (message.Contains("<@&747723060441776238>") || message.Contains("<@&785512028931489802>"))
                {
                    var emotes = e.Guild.Emojis.Values.ToList();
                    var index = Randomizer.GetRandomNumberBetween(0, emotes.Count);
                    await e.Message.CreateReactionAsync(emotes[index]);
                }

                if (message.Length > 0 && message[message.Length - 1] == '?')
                {
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

                    Utility.IncreaseRating(_config, user, 50, e);

                    return;
                }

                if (message.Contains("<@!760879629509853224>"))
                {
                    ReactToAppeal(e.Channel);
                    Utility.DecreaseRating(_config, user, 25, e);
                    return;
                }

                if (message.Contains("бот") || message.Contains("бинар") || message.Contains("двоич") ||
                    message.Contains("ошибка"))
                {
                    if (pidor < 6)
                    {
                        ReactToAppeal(e.Channel);
                        Utility.DecreaseRating(_config, user, 30, e);
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

                if (_config.IsPidor())
                {
                    var emojis = new List<DiscordEmoji>();
                    var megapidor = Randomizer.GetRandomNumberBetween(0, 50) == 0;
                    if (megapidor)
                    {
                        emojis.Add(Utility.Emoji(":regional_indicator_p:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_a:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_t:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_r:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_e:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_g:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_o:"));
                        emojis.Add(Utility.Emoji(":PATREGO:"));
                        Utility.IncreaseRating(_config, user, Randomizer.GetRandomNumberBetween(100, 200), e);
                    }
                    else
                    {
                        emojis.Add(Utility.Emoji(":regional_indicator_p:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_i:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_d:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_o:"));
                        emojis.Add(Utility.Emoji(":regional_indicator_r:"));
                        Utility.DecreaseRating(_config, user, Randomizer.GetRandomNumberBetween(35, 65), e);
                    }

                    foreach (var emoji in emojis)
                    {
                        await e.Message.CreateReactionAsync(emoji);
                        Thread.Sleep(150);
                    }

                    var count = megapidor ? 10 : 1;
                    for (var i = 0; i < count; i++)
                    {
                        _config.AddPidor(e.Author.Username);
                    }

                    if (_config.IsPidorOfTheDay(e.Author.Username))
                    {
                        var role = e.Guild.GetRole(781970998685466654);

                        if (_config.CurrentPidorOfTheDay == e.Author.Id) return;

                        if (_config.CurrentPidorOfTheDay != 0)
                        {
                            var currentPidor = await e.Guild.GetMemberAsync(_config.CurrentPidorOfTheDay);
                            await currentPidor.RevokeRoleAsync(role);
                        }

                        var newPidor = await e.Guild.GetMemberAsync(e.Author.Id);
                        try
                        {
                            await newPidor.GrantRoleAsync(role);
                        }
                        catch (Exception ex)
                        {
                            AddLog(ex.Message);
                        }

                        _config.CurrentPidorOfTheDay = newPidor.Id;
                    }

                    _lastPidorId = e.Author.Id;
                }

                if (randomMessageRating == -4)
                {
                    await e.Message.RespondAsync($"{Utility.StringEmoji(":osu:")}ждаю");
                }
            };

            _discord.TypingStarted += async (client, e) =>
            {
                var user = e.User;
                if (user.Id != _lastPidorId) return;

                var pool = new[]
                {
                    $"{user.Mention}, ты что-то хочешь мне высрать, пидор?",
                    $"{user.Mention}, пидорские возражения не принимаются!",
                    $"{user.Mention}, хочешь еще раз пидора получить что ли?",
                    $"{user.Mention}, а кто это тут такой маленький обиженный пидор хочет мне что-то высрать?"
                };
                await e.Channel.SendMessageAsync(pool[Randomizer.GetRandomNumberBetween(0, pool.Length)]);
                _lastPidorId = 0;
            };

            _discord.GuildMemberAdded += async (client, e) =>
            {
                if (e.Member.IsBot)
                {
                    await e.Guild.Channels[654674644590264340].SendMessageAsync(
                        $"Какого хуя на этом сервере существуют другие боты, кроме меня? " +
                        $"{Utility.StringEmoji(":pogOff:")} " +
                        $"{Utility.StringEmoji(":pogOff:")} " +
                        $"{Utility.StringEmoji(":pogOff:")} ");
                }
            };

            _discord.MessageReactionAdded += async (client, e) =>
            {
                var emoji = e.Emoji;
                var emojiName = emoji.Name;
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

                if ((emojiName == "peepoClown" || emojiName == "roflanClownbalo" ||
                     emojiName == "clownChamp") && e.User.Id == 423498706336088085)
                {
                    AddLog("Removed clown");
                    await e.Message.DeleteReactionAsync(emoji, e.User);
                }
                else if (Randomizer.GetRandomNumberBetween(1, 20) == 1)
                {
                    if (e.Message.Id != _lastReactionMessageId || e.User.Id != _lastReactionUserId ||
                        emojiName != _lastReactionEmote)
                    {
                        await e.Message.CreateReactionAsync(emoji);
                    }
                    else
                    {
                        await e.Message.RespondAsync(
                            $"{e.User.Mention}, зачем ты срешь эмотами? {Utility.StringEmoji(":peepoPooPoo:")}");
                    }
                }

                _lastReactionMessageId = e.Message.Id;
                _lastReactionUserId = e.User.Id;
                _lastReactionEmote = emojiName;
            };

            _discord.MessageReactionRemoved += (client, e) =>
            {
                var emoji = e.Emoji;
                var emojiName = emoji.Name;
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

                return Task.CompletedTask;
            };

            _discord.PresenceUpdated += (client, args) =>
            {
                var user = args.User;
                var name = args.Activity.Name;
                AddLog($"User {user.Username} is {args.Activity.ActivityType} {name}");

                return Task.CompletedTask;
            };

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

        private void CheckStats(string message = "default zalupa")
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

        private void Exit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Environment.Exit(0);
        }

        private static async void AddLog(string content)
        {
            using (var file = new StreamWriter("log.txt", true))
            {
                await file.WriteLineAsync($"{DateTime.Now} | {content}");
            }
        }
    }
}