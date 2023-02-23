using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

[ObsoleteCommand("/roles -a -n -g -r")]
public class CreateRoleCommand : ICommand
{
    public CreateRoleCommand(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IUserBalanceService userBalanceService,
        IGuildSettingsService guildSettingsService,
        IPartyService partyService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.userBalanceService = userBalanceService;
        this.guildSettingsService = guildSettingsService;
        this.partyService = partyService;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        var guild = await discordClientWrapper.Guilds.GetGuildAsync();

        var userId = e.Author.Id;
        var apiUser = await apiClient.Users.RatingAsync(userId);
        var serverMember = await discordClientWrapper.Members.GetAsync(userId);

        var args = e.Message.Content.TrimEnd().Split();
        var serverRoles = guild.Roles;
        if (args.Length == 1)
        {
            var joinableRoles = partyService.PartiesInfo.JoinableRoles.Select(id => serverRoles[id]);
            await discordClientWrapper.Messages.RespondAsync(
                e.Message,
                $"Доступные роли:\n```\n{string.Join("\n", joinableRoles.Select(role => $"{role.Id} - {role.Name}"))}\n```"
            );
            return;
        }

        if (args.Length == 2)
        {
            var isRoleIdValid = ulong.TryParse(args[1], out var roleId);
            if (isRoleIdValid && serverRoles.ContainsKey(roleId))
            {
                var memberRole = serverMember.Roles.FirstOrDefault(role => role.Id == roleId);
                if (memberRole != null)
                {
                    await discordClientWrapper.Roles.RevokeRoleAsync(userId, serverMember.Roles.First(role => role.Id == roleId));
                    await discordClientWrapper.Messages.RespondAsync(e.Message, $"Убрал роль {memberRole.Name}");
                    return;
                }

                if (apiUser.ScamCoins < guildSettings.JoinRolePrice)
                {
                    await discordClientWrapper.Messages.RespondAsync(
                        e.Message,
                        $"Для присоединения к роли нужно отдать {guildSettings.JoinRolePrice} скамкойнов"
                    );
                    return;
                }

                var role = serverRoles[roleId];

                await discordClientWrapper.Roles.GrantRoleAsync(userId, role);
                await discordClientWrapper.Messages.RespondAsync(e.Message, $"Выдал роль {role.Name}");
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(userId, -guildSettings.JoinRolePrice, $"Выдача роли {role.Name}");
                return;
            }
        }

        if (apiUser.ScamCoins < guildSettings.CreateRolePrice)
        {
            await discordClientWrapper.Messages.RespondAsync(
                e.Message,
                $"Для создания новой роли нужно отдать {guildSettings.CreateRolePrice} скамкойнов"
            );
            return;
        }

        try
        {
            var newRoleName = string.Join(" ", args.Skip(1));
            var newRole = await discordClientWrapper.Roles.CreateNewRoleAsync(newRoleName);
            await discordClientWrapper.Roles.GrantRoleAsync(userId, newRole);
            await discordClientWrapper.Messages.RespondAsync(e.Message, $"Создал роль {newRole.Name} и выдал ее создателю");
            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(userId, -guildSettings.CreateRolePrice, $"Создание роли {newRoleName}");
            partyService.PartiesInfo.JoinableRoles.Add(newRole.Id);
            partyService.Save();
        }
        catch (Exception ex)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, ex.Message);
        }
    }

    public Task<string> Help()
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        var result = $"{guildSettings.CommandsPrefix}{Name} (без аргументов) - список всех ролей, к которым можно присоединиться\n" +
                     $"{guildSettings.CommandsPrefix}{Name} [Название] - создать роль ({guildSettings.CreateRolePrice} скамкойнов)\n" +
                     $"{guildSettings.CommandsPrefix}{Name} [ID] - присоединиться к роли ({guildSettings.JoinRolePrice} скамкойнов) или покинуть ее";

        return Task.FromResult(result);
    }

    public string Name => "role";
    public bool IsObsolete => false;

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IUserBalanceService userBalanceService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IPartyService partyService;
}