using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.Helpers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands
{
    public class IpCommand : BaseCommand
    {
        public IpCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public override async void Execute(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            var parts = message.Split(' ');
            var needPing = parts.Length > 1 && parts[1] == "ping";
            var respondMessage = await e.Message.RespondAsync($"{Utility.StringEmoji(":pauseChamp:")}");
            using var tcpClient = new TcpClient();
            try
            {
                const string serverPath = "C:\\Minecraft\\Server 1.17 Clear";
                var isModded = Directory.Exists($"{serverPath}\\mods");
                const string serverDescription = "Версия: 1.17 vanilla";
                var ip = await GetRealIp();
                var mods = new List<string>();
                if (isModded)
                {
                    var modDir = $"{serverPath}\\mods";
                    mods = Directory.GetFiles(modDir).Select(file => file[(modDir.Length + 1)..]).ToList();
                }
                tcpClient.SendTimeout = 1000;
                tcpClient.ReceiveTimeout = 1000;
                await tcpClient.ConnectAsync("192.168.1.134", 1505);
                var messageContent = $"IP: {ip}:1505\n{serverDescription}";
                if (isModded)
                {
                    messageContent += $"\nУстановленные моды:\n{string.Join("\n", mods)}";
                }

                await respondMessage.ModifyAsync(messageContent);
            }
            catch (Exception)
            {
                if (!needPing)
                {
                    await respondMessage.ModifyAsync("Сервер не запущен");
                    return;
                }

                var admin = await e.Guild.GetMemberAsync(259306088040628224);
                await respondMessage.ModifyAsync($"Сервер не запущен\n{admin.Mention} запусти сервак!!!");
            }
        }

        private static async Task<string> GetRealIp()
        {
            const string apiUrl = "https://api.ipify.org";
            var request = (HttpWebRequest)WebRequest.Create(apiUrl);
            var response = (HttpWebResponse)request.GetResponse();

            using var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return await stream.ReadToEndAsync();
        }

        private static async Task<string> GetNgrokIpFromFile(string serverDirectory) =>
            await File.ReadAllTextAsync($"{serverDirectory}\\server.txt");

        public override string Help()
        {
            return "Получения айпишника для удовлетворения кубоёбов\nИспользование:\n" +
                   "!ip [ping] (параметр ping нужен, чтобы пингануть админа, чтобы он запустил сервак";
        }
    }
}