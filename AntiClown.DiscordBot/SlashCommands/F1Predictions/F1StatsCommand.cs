using AntiClown.DiscordBot.EmbedBuilders.F1PredictionsStats;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1Stats_Group, "Статистика по предсказаниям")]
public class F1StatsCommand : SlashCommandModuleWithMiddlewares
{
    public F1StatsCommand(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IF1PredictionStatsEmbedBuilder f1PredictionStatsEmbedBuilder,
        ICommandExecutor commandExecutor
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.f1PredictionStatsEmbedBuilder = f1PredictionStatsEmbedBuilder;
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Stats_MostPickedDriversByUsers, "Самые выбираемые гонщики")]
    public async Task GetMostPickedDriversByUsers(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var mostPickedDriversByUsers = await antiClownEntertainmentApiClient.F1PredictionsStats.GetMostPickedDriversByUsersAsync();
                var embed = f1PredictionStatsEmbedBuilder.Build(mostPickedDriversByUsers);
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IF1PredictionStatsEmbedBuilder f1PredictionStatsEmbedBuilder;
}