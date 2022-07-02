using System.Net;
using System.Net.Sockets;
using System.Text;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands
{
    public class IpCommand : ICommand
    {
        public IpCommand(
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.guildSettingsService = guildSettingsService;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
            var message = e.Message.Content;
            var parts = message.Split(' ');
            var needPing = parts.Length > 1 && parts[1] == "ping";
            var respondMessageContent = await discordClientWrapper.Emotes.FindEmoteAsync("pauseChamp");
            var respondMessage = await discordClientWrapper.Messages.RespondAsync(e.Message, respondMessageContent);
            using var tcpClient = new TcpClient();
            try
            {
                var serverPath = guildSettings.MinecraftServerFolder;
                var isModded = Directory.Exists($"{serverPath}\\mods");
                const string serverDescription = "Версия: 1.19";
                var ip = await GetRealIp();
                var mods = new List<string>();
                if (isModded)
                {
                    var modDir = $"{serverPath}\\mods";
                    mods = Directory.GetFiles(modDir).Select(file => file[(modDir.Length + 1)..]).ToList();
                }

                tcpClient.SendTimeout = 1000;
                tcpClient.ReceiveTimeout = 1000;
                await tcpClient.ConnectAsync(guildSettings.MinecraftServerLocalAddress, guildSettings.MinecraftServerPort);
                var messageContent = $"IP: {ip}:{guildSettings.MinecraftServerPort}\n{serverDescription}";
                if (isModded)
                {
                    messageContent += $"\nУстановленные моды:\n{string.Join("\n", mods)}";
                }

                await discordClientWrapper.Messages.ModifyAsync(respondMessage, messageContent);
            }
            catch (Exception)
            {
                if (!needPing)
                {
                    await discordClientWrapper.Messages.ModifyAsync(respondMessage, "Сервер не запущен");
                    return;
                }

                var admin = await e.Guild.GetMemberAsync(guildSettings.AdminId);
                await discordClientWrapper.Messages.ModifyAsync(respondMessage, $"Сервер не запущен\n{admin.Mention} запусти сервак!!!");
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

        public Task<string> Help()
        {
            return Task.FromResult("Получение айпишника для удовлетворения кубоёбов\nИспользование:\n" +
                                   $"{guildSettingsService.GetGuildSettings().CommandsPrefix}{Name} [ping] " +
                                   $"(параметр ping нужен, чтобы пингануть админа, чтобы он запустил сервак");
        }

        public string Name => "ip";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IGuildSettingsService guildSettingsService;
    }
}