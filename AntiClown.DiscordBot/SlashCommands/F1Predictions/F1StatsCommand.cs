using AntiClown.DiscordBot.Cache.Users;
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
        IUsersCache usersCache,
        ICommandExecutor commandExecutor
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.f1PredictionStatsEmbedBuilder = f1PredictionStatsEmbedBuilder;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Stats_MostPickedDrivers, "Самые выбираемые гонщики среди всех участников предсказаний")]
    public async Task GetMostPickedDrivers(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var mostPickedDriversByUsers = await antiClownEntertainmentApiClient.F1PredictionsStats.GetMostPickedDriversAsync();
                var embed = f1PredictionStatsEmbedBuilder.Build(mostPickedDriversByUsers);
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Stats_MostPickedDriversByUser, "Твои самые выбираемые гонщики")]
    public async Task GetMostPickedDriversByUser(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(interactionContext.Member.Id);
                var mostPickedDriversByUsers = await antiClownEntertainmentApiClient.F1PredictionsStats.GetMostPickedDriversAsync(apiUserId);
                var embed = f1PredictionStatsEmbedBuilder.Build(mostPickedDriversByUsers);
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Stats_MostProfitableDrivers, "\"Правильные ответы\" на предсказания")]
    public async Task GetMostProfitableDrivers(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var mostPickedDriversByUsers = await antiClownEntertainmentApiClient.F1PredictionsStats.GetMostProfitableDriversAsync();
                var embed = f1PredictionStatsEmbedBuilder.Build(mostPickedDriversByUsers);
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Stats_UserPointsStats, "Среднее и медианное количество очков за гонку")]
    public async Task GetUserPointsStats(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(interactionContext.Member.Id);
                var userPointsStats = await antiClownEntertainmentApiClient.F1PredictionsStats.GetUserPointsStats(apiUserId);
                var embed = f1PredictionStatsEmbedBuilder.Build(userPointsStats);
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IF1PredictionStatsEmbedBuilder f1PredictionStatsEmbedBuilder;
    private readonly IUsersCache usersCache;
}