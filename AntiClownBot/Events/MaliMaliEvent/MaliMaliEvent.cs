using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;

namespace AntiClownBot.Events.MaliMaliEvent
{
    public class MaliMaliEvent : BaseEvent
    {
        public override int EventCooldown => 10 * 60 * 1000;

        public override async void ExecuteAsync()
        {
            Config.EventPossibleTimes["malimali"] = DateTime.Now.AddMilliseconds(EventCooldown);
            var channels = DiscordClient.Guilds[277096298761551872].Channels.Values
                .Where(ch => ch.Type == ChannelType.Voice && ch.Users.ToList().Count > 0).ToList();
            if (channels.Count == 0)
            {
                SendMessageToChannel($"Никто не пришел на MALI-MALI-фанвстречу {Utility.StringEmoji(":BibleThump:")}");
                return;
            }

            TellBackStory();

            var channel = channels.SelectRandomItem();
            if (!Voice.TryConnect(channel, out var vnc))
            {
                vnc.Disconnect();
                Voice.TryConnect(channel, out vnc);
            }
            
            try
            {
                Voice.StopPlaying();
                while (vnc.IsPlaying) await vnc.WaitForPlaybackFinishAsync();
                Voice.PlaySound("zapret.mp3");

                var voiceUsers = DiscordClient.Guilds[277096298761551872].VoiceStates.Where(kvp =>
                        kvp.Value?.User != null &&
                        kvp.Value.Channel is not null &&
                        !kvp.Value.User.IsBot &&
                        kvp.Value.Channel.Id == channel.Id)
                    .ToList();
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

                        var member = await DiscordClient
                            .Guilds[277096298761551872].GetMemberAsync(key);
                        await member.ModifyAsync(model => model.Muted = false);

                        if (member.IsBot) continue;
                        var socialUser = Config.Users.ContainsKey(value.User.Id)
                            ? Config.Users[value.User.Id]
                            : new SocialRatingUser(member.Id, member.Username);
                        socialUser.ChangeRating(Randomizer.GetRandomNumberBetween(100, 150));
                    }
                }).Start();

                Voice.PlaySound("malimali.mp3");
            }
            catch (Exception ex)
            {
                NLogWrapper.GetDefaultLogger().Error(ex.Message);
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