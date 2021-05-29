using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;

namespace AntiClownBot.Events.MaliMaliEvent
{
    public class MaliMaliEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
            var channels = DiscordClient.Guilds[277096298761551872].Channels.Values
                .Where(ch => ch.Type == ChannelType.Voice && ch.Users.ToList().Count > 0).ToList();
            if (channels.Count == 0)
            {
                SendMessageToChannel($"Никто не пришел на MALI-MALI-фанвстречу {Utility.StringEmoji(":BibleThump:")}");
                return;
            }

            TellBackStory();

            var channel = channels.SelectRandomItem();
            Voice.TryConnect(channel, out var vnc);

            try
            {
                Voice.PlaySound("zapret.mp3");

                var voiceUsers = DiscordClient.Guilds[277096298761551872].VoiceStates
                    .Where(kvp => kvp.Value.Channel.Id == channel.Id).ToList();
                foreach (var user in voiceUsers.Where(u => u.Value.User.Id != Constants.BotId))
                {
                    if (user.Value.Channel == channel)
                    {
                        var member = DiscordClient
                            .Guilds[277096298761551872].GetMemberAsync(user.Key);
                        await member.Result.ModifyAsync(model => model.Muted = true);
                    }
                }

                new Thread(async () =>
                {
                    var voiceChannel = channel;
                    await Task.Delay(60 * 1000);
                    foreach (var (key, value) in voiceUsers.Where(u => u.Value.User.Id != Constants.BotId))
                    {
                        if (value.Channel != voiceChannel) continue;

                        var member = DiscordClient
                            .Guilds[277096298761551872].GetMemberAsync(key);
                        await member.Result.ModifyAsync(model => model.Muted = false);

                        var socialUser = Config.Users[value.User.Id];
                        socialUser.ChangeRating(Randomizer.GetRandomNumberBetween(100, 150));
                    }
                }).Start();

                Voice.PlaySound("malimali.mp3");
            }
            catch (Exception ex)
            {
                NLogWrapper.GetDefaultLogger().Error(ex.Message);
            }
            finally
            {
                Voice.Disconnect();
            }
        }

        protected override string BackStory()
        {
            return
                $"{Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")} MALI MALI {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")}\n" +
                $"{Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")} MALI MALI {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")}\n" +
                $"{Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")} MALI MALI {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")}\n" +
                $"{Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")} MALI MALI {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")}\n" +
                $"{Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")} MALI MALI {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")}\n" +
                $"{Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")} MALI MALI {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":pomLeft:")} {Utility.StringEmoji(":FLOPPA:")} {Utility.StringEmoji(":pomRight:")} {Utility.StringEmoji(":pomRight:")}";
        }
    }
}