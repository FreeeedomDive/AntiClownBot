using System.Net.Sockets;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using CommonServices.IpService;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other.Ip;

public class IpCommandModule : ApplicationCommandModule
{
    public IpCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IGuildSettingsService guildSettingsService,
        IIpService ipService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.guildSettingsService = guildSettingsService;
        this.ipService = ipService;
    }

    [SlashCommand("ip", "Узнать адрес майнкрафт-сервера")]
    public async Task GetServerIp(
        InteractionContext context,
        [Option("ping", "Призвать админа, чтобы он запустил сервак?")]
        Ping? ping = null
    )
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        var respondMessageContent = await discordClientWrapper.Emotes.FindEmoteAsync("HACKERMANS");
        var respondMessage = await discordClientWrapper.Messages.RespondAsync(context, respondMessageContent);
        using var tcpClient = new TcpClient();
        try
        {
            var serverPath = guildSettings.MinecraftServerFolder;
            var isModded = Directory.Exists($"{serverPath}\\mods");
            const string serverDescription = "Версия: 1.19";
            var ip = await ipService.GetIp();
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
            if (ping is null)
            {
                await discordClientWrapper.Messages.ModifyAsync(respondMessage, "Сервер не запущен");
                return;
            }

            var admin = await context.Guild.GetMemberAsync(guildSettings.AdminId);
            await discordClientWrapper.Messages.ModifyAsync(respondMessage,
                $"Сервер не запущен\n{admin.Mention} запусти сервак!!!");
        }
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IIpService ipService;
}