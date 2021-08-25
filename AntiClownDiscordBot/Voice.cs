using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AntiClownBot.Helpers;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace AntiClownBot
{
    static class Voice
    {
        public static VoiceNextConnection Connection;
        public static VoiceNextExtension VoiceExtension;
        private static object locker = new();
        public static VoiceTransmitSink TxStream;
        public static Stream FfOut;

        public static Queue<string> SoundQueue = new Queue<string>();

        public static bool TryConnect(DiscordChannel channel, out VoiceNextConnection connection)
        {
            connection = VoiceExtension.GetConnection(Utility.Client.Guilds[277096298761551872]);
            if (connection != null)
            {
                return false;
            }
            connection = VoiceExtension.ConnectAsync(channel).Result;
            Connection = connection;
            return true;
        }
        public static async void PlaySound(string soundname)
        {
            var vnc = VoiceExtension.GetConnection(Utility.Client.Guilds[277096298761551872]);
            if (vnc == null)
            {
                NLogWrapper.GetDefaultLogger().Info($"Бот не подключен");
                return;
            }

            if (!File.Exists(soundname))
            {
                NLogWrapper.GetDefaultLogger().Info($"Нет файла {soundname}");
                return;
            }

            lock (locker)
            {
                if (vnc.IsPlaying)
                {
                    NLogWrapper.GetDefaultLogger().Info($"Добавлен в очередь {soundname}");
                    SoundQueue.Enqueue(soundname);
                    return;
                }
            }

            Exception exc = null;
            NLogWrapper.GetDefaultLogger().Info($"Начинаем проигрывать {soundname}");

            try
            {
                await vnc.SendSpeakingAsync(true);

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{soundname}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var ffmpeg = Process.Start(psi);
                FfOut = ffmpeg.StandardOutput.BaseStream;

                TxStream = vnc.GetTransmitSink();
                await FfOut.CopyToAsync(TxStream);
                await TxStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();
            }
            catch (Exception ex)
            {
                exc = ex;
            }
            finally
            {
                await vnc.SendSpeakingAsync(false);
                NLogWrapper.GetDefaultLogger().Info($"Закончили проигрывать {soundname}");
                if (exc != null)
                    NLogWrapper.GetDefaultLogger().Info($"Выскочило исключение {exc.GetType()}: {exc.Message}");
                lock (locker)
                {
                    if (SoundQueue.Count > 0)
                    {
                        NLogWrapper.GetDefaultLogger().Info($"Играю некст трек");
                        PlaySound(SoundQueue.Dequeue());
                    }
                    else
                    {
                        NLogWrapper.GetDefaultLogger().Info($"Ливну через минуту");
                        Disconnect();
                    }
                }
            }
        }

        public static void StopPlaying()
        {
            SoundQueue.Clear();
        }

        public static async void Disconnect()
        {
            var vnc = VoiceExtension.GetConnection(Utility.Client.Guilds[277096298761551872]);
            if (vnc == null)
            {
                NLogWrapper.GetDefaultLogger().Info($"Бот не подключен");
                return;
            }

            await Task.Delay(60*1000);
            lock (locker)
            {
                if (SoundQueue.Count > 0 && !vnc.IsPlaying)
                {
                    PlaySound(SoundQueue.Dequeue());
                    return;
                }
            }

            if (vnc.IsPlaying)
            {
                NLogWrapper.GetDefaultLogger().Info($"Бля, чёт играет, ливать не буду");
                return;
            }
            NLogWrapper.GetDefaultLogger().Info($"Ливаю");
            vnc.Disconnect();
        }
    }
}
