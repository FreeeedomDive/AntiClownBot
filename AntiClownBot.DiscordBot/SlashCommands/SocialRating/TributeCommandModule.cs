using AntiClownApiClient;
using AntiClownDiscordBotVersion2.Models;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.SocialRating;

public class TributeCommandModule : SlashCommandModuleWithMiddlewares
{
    public TributeCommandModule(
        ICommandExecutor commandExecutor,
        IApiClient apiClient,
        TributeService tributeService
    ) : base(commandExecutor)
    {
        this.apiClient = apiClient;
        this.tributeService = tributeService;
    }

    [SlashCommand(Interactions.Commands.Tribute, "Подношение императору XI")]
    public async Task Tribute(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            var tributeResult = await apiClient.Users.TributeAsync(context.User.Id);
            var tributeEmbed = await tributeService.TryMakeEmbedForTribute(tributeResult);
            if (tributeEmbed != null)
            {
                await RespondToInteractionAsync(context, tributeEmbed);
            }
        });
    }

    private readonly IApiClient apiClient;
    private readonly TributeService tributeService;
}