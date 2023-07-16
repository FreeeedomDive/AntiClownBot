using AntiClown.Api.Client;
using AntiClown.Api.Dto.Exceptions.Tribute;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.Tributes;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.SocialRating;

public class TributeCommandModule : SlashCommandModuleWithMiddlewares
{
    public TributeCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        ITributeEmbedBuilder tributeEmbedBuilder,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.tributeEmbedBuilder = tributeEmbedBuilder;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Tribute, "Подношение императору XI")]
    public async Task Tribute(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                try
                {
                    var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                    var tributeResult = await antiClownApiClient.Tribute.TributeAsync(userId);
                    var tributeEmbed = await tributeEmbedBuilder.BuildForSuccessfulTributeAsync(tributeResult);
                    await RespondToInteractionAsync(context, tributeEmbed);
                }
                catch (TributeIsOnCooldownException)
                {
                    var failedTributeEmbed = await tributeEmbedBuilder.BuildForTributeOnCoolDownAsync();
                    await RespondToInteractionAsync(context, failedTributeEmbed);
                }
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ITributeEmbedBuilder tributeEmbedBuilder;
    private readonly IUsersCache usersCache;
}