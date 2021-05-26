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
            var vnext = Utility.Voice;
            var vnc = vnext.GetConnection(DiscordClient.Guilds[277096298761551872]);
            if (vnc != null)
            {
                return;
            }

            vnc = await vnext.ConnectAsync(channel);

            try
            {
                await PlaySound("zapret.mp3");

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

                await PlaySound("malimali.mp3");
            }
            catch (Exception ex)
            {
                NLogWrapper.GetDefaultLogger().Error(ex.Message);
            }
            finally
            {
                await vnc.SendSpeakingAsync(false);
                vnc.Disconnect();
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

        private static async Task PlaySound(string filename)
        {
            var vnext = Utility.Voice;
            var vnc = vnext.GetConnection(Utility.Client.Guilds[277096298761551872]);
            if (vnc == null)
            {
                NLogWrapper.GetDefaultLogger().Info($"Бот не подключен");
                return;
            }

            if (!File.Exists(filename))
            {
                NLogWrapper.GetDefaultLogger().Info($"Нет файла {filename}");
                return;
            }

            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();
            Exception exc = null;
            NLogWrapper.GetDefaultLogger().Info($"Начинаем проигрывать {filename}");

            try
            {
                await vnc.SendSpeakingAsync(true);

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{filename}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                var txStream = vnc.GetTransmitSink();
                await ffout.CopyToAsync(txStream);
                await txStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();
            }
            catch (Exception ex)
            {
                exc = exc;
            }
            finally
            {
                await vnc.SendSpeakingAsync(false);
                NLogWrapper.GetDefaultLogger().Info($"Закончили проигрывать {filename}");
            }

            if (exc != null)
                NLogWrapper.GetDefaultLogger().Info($"Выскочило исключение {exc.GetType()}: {exc.Message}");
        }
    }
}