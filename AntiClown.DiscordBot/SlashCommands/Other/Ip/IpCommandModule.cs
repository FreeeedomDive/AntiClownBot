using AntiClown.DiscordBot.SlashCommands.Base;

namespace AntiClown.DiscordBot.SlashCommands.Other.Ip;

public class IpCommandModule : SlashCommandModuleWithMiddlewares
{
    public IpCommandModule(
        ICommandExecutor commandExecutor
    ) : base(commandExecutor)
    {
    }

    /*
    TODO: forgotten until better times
    [SlashCommand(InteractionsIds.CommandsNames.Ip, "Узнать адрес майнкрафт-сервера")]
    public async Task GetServerIp(
        InteractionContext context,
        [Option("ping", "Призвать админа, чтобы он запустил сервак?")]
        Ping? ping = null
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
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
                await tcpClient.ConnectAsync(guildSettings.MinecraftServerLocalAddress,
                    guildSettings.MinecraftServerPort);
                var messageContent = $"IP: {ip}:{guildSettings.MinecraftServerPort}\n{serverDescription}";
                if (isModded)
                {
                    messageContent += $"\nУстановленные моды:\n{string.Join("\n", mods)}";
                }

                await RespondToInteractionAsync(context, messageContent);
            }
            catch (Exception)
            {
                if (ping is null)
                {
                    await RespondToInteractionAsync(context, "Сервер не запущен");
                    return;
                }

                var admin = await context.Guild.GetMemberAsync(guildSettings.AdminId);
                await RespondToInteractionAsync(context, $"Сервер не запущен\n{admin.Mention} запусти сервак!!!");
            }
        });
    }*/
}