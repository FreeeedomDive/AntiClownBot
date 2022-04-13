using System;
using System.Linq;
using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands;

public class CreateRoleCommand : BaseCommand
{
    public CreateRoleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
    {
    }

    public override async void Execute(MessageCreateEventArgs e)
    {
        var userId = e.Author.Id;
        var apiUser = UsersApi.Rating(userId);
        var serverMember = Configuration.GetServerMember(userId);
        var args = e.Message.Content.TrimEnd().Split();
        var serverRoles = e.Guild.Roles;
        if (args.Length == 1)
        {
            var joinableRoles = Config.JoinableRoles.Select(id => serverRoles[id]);
            await e.Message.RespondAsync($"Доступные роли:\n```\n{string.Join("\n", joinableRoles.Select(role => $"{role.Id} - {role.Name}"))}\n```");
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
                    await serverMember.RevokeRoleAsync(serverMember.Roles.First(role => role.Id == roleId));
                    await e.Message.RespondAsync($"Убрал роль {memberRole.Name}");
                    return;
                }

                if (apiUser.ScamCoins < JoinRolePrice)
                {
                    await e.Message.RespondAsync($"Для присоединения к роли нужно отдать {JoinRolePrice} скамкойнов");
                    return;
                }

                var role = serverRoles[roleId];

                await serverMember.GrantRoleAsync(role);
                await e.Message.RespondAsync($"Выдал роль {role.Name}");
                Config.ChangeBalance(userId, -JoinRolePrice, $"Выдача роли {role.Name}");
                return;
            }
        }

        if (apiUser.ScamCoins < CreateRolePrice)
        {
            await e.Message.RespondAsync($"Для создания новой роли нужно отдать {CreateRolePrice} скамкойнов");
            return;
        }

        try
        {
            var newRoleName = string.Join(" ", args.Skip(1));
            var newRole = await e.Guild.CreateRoleAsync(newRoleName);
            await serverMember.GrantRoleAsync(newRole);
            await e.Message.RespondAsync($"Создал роль {newRole.Name} и выдал ее создателю");
            Config.ChangeBalance(userId, -CreateRolePrice, $"Создание роли {newRoleName}");
            Config.JoinableRoles.Add(newRole.Id);
            Config.Save();
        }
        catch (Exception ex)
        {
            await e.Message.RespondAsync(ex.Message);
        }
    }

    public override string Help() =>
        "!role (без аргументов) - список всех ролей, к которым можно присоединиться\n" +
        $"!role [Название] - создать роль ({CreateRolePrice} скамкойнов)\n" +
        $"!role [ID] - присоединиться к роли ({JoinRolePrice} скамкойнов) или покинуть ее";

    private const int JoinRolePrice = 1000;
    private const int CreateRolePrice = 2500;
}