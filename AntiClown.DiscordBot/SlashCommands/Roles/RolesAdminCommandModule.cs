using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Roles.Repositories;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Roles;

[SlashCommandGroup(InteractionsIds.CommandsNames.RolesAdmin_Group, "Админское управление командами", false)]
[SlashCommandPermissions(Permissions.ViewAuditLog)]
public class RolesAdminCommandModule : SlashCommandModuleWithMiddlewares
{
    public RolesAdminCommandModule(
        ICommandExecutor commandExecutor,
        IRolesRepository rolesRepository
    ) : base(commandExecutor)
    {
        this.rolesRepository = rolesRepository;
    }

    [SlashCommand(InteractionsIds.CommandsNames.RolesAdmin_Add, "Добавить роль")]
    public async Task ListRolesAsync(
        InteractionContext context,
        [Option("role", "Роль")] DiscordRole role
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var exists = await rolesRepository.ExistsAsync(role.Id);
                if (exists)
                {
                    await RespondToInteractionAsync(context, "Роль уже добавлена в список ролей, к которым можно присоединиться");
                    return;
                }

                await rolesRepository.CreateAsync(role.Id);
                await RespondToInteractionAsync(context, "Роль добавлена в список");
            }
        );
    }

    private readonly IRolesRepository rolesRepository;
}