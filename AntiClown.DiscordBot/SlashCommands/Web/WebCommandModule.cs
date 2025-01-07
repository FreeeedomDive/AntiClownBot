using AntiClown.Data.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.SlashCommands.Web;

public class WebCommandModule(
    IAntiClownDataApiClient antiClownDataApiClient,
    IUsersCache usersCache,
    IOptions<WebOptions> webOptions,
    ICommandExecutor commandExecutor
)
    : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.Web, "Открыть страницу в браузере")]
    public async Task Web(InteractionContext context)
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var token = await antiClownDataApiClient.Tokens.GetAsync(apiUserId);
                var embed = new DiscordEmbedBuilder()
                            .WithTitle("Доступ в веб-версию бота")
                            .AddField("Адрес", webOptions.Value.FrontApplicationUrl)
                            .AddField("UserId", apiUserId.ToString())
                            .AddField("Token", token)
                            .Build();

                await RespondToInteractionAsync(context, embed);
            }
        );
    }
}