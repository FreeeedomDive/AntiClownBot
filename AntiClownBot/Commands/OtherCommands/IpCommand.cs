using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands
{
    public class IpCommand: BaseCommand
    {
        public IpCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var message = e.Message.Content;
            var parts = message.Split(' ');
            var needPing = parts.Length > 1 && parts[1] == "ping";
            var respondMessage = await e.Message.RespondAsync($"{Utility.StringEmoji(":pauseChamp:")}");
            using var tcpClient = new TcpClient();
            try
            {
                var ip = await File.ReadAllTextAsync("C:\\Minecraft\\Server 1.17 Clear\\server.txt");
                tcpClient.SendTimeout = 1000;
                tcpClient.ReceiveTimeout = 1000;
                await tcpClient.ConnectAsync("localhost", 25565);
                await respondMessage.ModifyAsync($"IP: {ip}\nСервер запущен");
            } catch (Exception) {
                if (!needPing)
                {
                    await respondMessage.ModifyAsync("Сервер не запущен");
                    return;
                }

                var admin = await e.Guild.GetMemberAsync(259306088040628224);
                await respondMessage.ModifyAsync($"Сервер не запущен\n{admin.Mention} запусти сервак!!!");
            }
        }

        public override string Help()
        {
            return "Получения айпишника для удовлетворения кубоёбов\nИспользование:\n" +
                   "!ip [ping] (параметр ping нужен, чтобы пингануть админа, чтобы он запустил сервак";
        }
    }
}