using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace AntiClownBot
{
    static class Voice
    {
        public static VoiceNextExtension VoiceExtension;
        private static object locker;

        public static bool TryConnect(DiscordChannel channel, out VoiceNextConnection connection)
        {
            connection = VoiceExtension.GetConnection(Utility.Client.Guilds[277096298761551872]);
            if (connection != null)
            {
                return false;
            }
            connection = VoiceExtension.ConnectAsync(channel).Result;
            return true;
        }

        private static Queue<string> _soundQueue = new Queue<string>();
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
                    _soundQueue.Enqueue(soundname);
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
                var ffout = ffmpeg.StandardOutput.BaseStream;

                var txStream = vnc.GetTransmitSink();
                await ffout.CopyToAsync(txStream);
                await txStream.FlushAsync();
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
                    if (_soundQueue.Count > 0)
                    {
                        PlaySound(_soundQueue.Dequeue());
                    }
                }

            }

        }

        public static async void Disconnect()
        {
            var vnc = VoiceExtension.GetConnection(Utility.Client.Guilds[277096298761551872]);
            if (vnc == null)
            {
                NLogWrapper.GetDefaultLogger().Info($"Бот не подключен");
                return;
            }

            while (vnc.IsPlaying)
            {
                await vnc.WaitForPlaybackFinishAsync();
            }
            vnc.Disconnect();
        }
    }
}
