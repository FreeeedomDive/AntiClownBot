using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

public class UsersCommandModule(ICommandExecutor commandExecutor, IUsersCache usersCache) : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.Dev_GetApiId, "Получить api id челика с сервера", false)]
    [SlashCommandPermissions(Permissions.Administrator)]
    public async Task GetMemberApiId(InteractionContext context, [Option("member", "Участник сервера")] DiscordUser member)
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                var apiId = await usersCache.GetApiIdByMemberIdAsync(member.Id);
                await RespondToInteractionAsync(context, apiId.ToString());
            }
        );
    }
}