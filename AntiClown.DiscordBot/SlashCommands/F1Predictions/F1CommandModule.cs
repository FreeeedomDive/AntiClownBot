using System.Text;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.F1Predictions;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1_Group, "Команды, связанные с гонками Ф1")]
public class F1CommandModule : SlashCommandModuleWithMiddlewares
{
    public F1CommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IInteractivityRepository interactivityRepository,
        IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.interactivityRepository = interactivityRepository;
        this.f1PredictionsEmbedBuilder = f1PredictionsEmbedBuilder;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1_List, "Показать текущие предсказания")]
    public async Task ListPredictions(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(currentRace.Details!.RaceId);
                if (race.Predictions.Count == 0)
                {
                    await RespondToInteractionAsync(interactionContext, "Никто не вносил свои предсказания");
                    return;
                }

                var embed = f1PredictionsEmbedBuilder.BuildPredictionsList(race);
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1_Standings, "Показать таблицу очков за предсказания")]
    public async Task Standings(
        InteractionContext interactionContext,
        [Option("season", "Сезон (по умолчанию текущий год)")]
        long? season = null
    )
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var standings = await antiClownEntertainmentApiClient.F1Predictions.ReadStandingsAsync(
                    season is null ? DateTime.UtcNow.Year : (int)season
                );
                if (standings.Count == 0)
                {
                    await RespondToInteractionAsync(interactionContext, $"В сезоне {season} еще не было ни одной гонки");
                    return;
                }

                var userToMember = standings.Keys.ToDictionary(x => x, x => usersCache.GetMemberByApiIdAsync(x).GetAwaiter().GetResult());
                var longestNameLength = userToMember.Values.Select(x => x.ServerOrUserName().Length).Max();
                var stringBuilder = new StringBuilder("```\n");
                var predictionsTable = standings
                                       .Select(
                                           kv => new
                                           {
                                               UserId = kv.Key,
                                               Predictions = kv.Value,
                                               TotalPoints = kv.Value.Select(
                                                   p => p is null
                                                       ? 0
                                                       : SumPoints(p)
                                               ).Sum(),
                                           }
                                       )
                                       .OrderByDescending(x => x.TotalPoints);
                var position = 1;
                foreach (var userPredictions in predictionsTable)
                {
                    var member = userToMember[userPredictions.UserId];
                    var userName = member.ServerOrUserName().AddSpaces(longestNameLength, false);
                    stringBuilder
                        .Append($"{position++} | {userName} | ")
                        .Append(
                            string.Join(
                                " ", userPredictions
                                     .Predictions
                                     .Select(p => p is null ? "  " : (SumPoints(p)).ToString().AddSpaces(2))
                            )
                        )
                        .Append($" | {userPredictions.TotalPoints.AddSpaces(3)}")
                        .Append($" | {userPredictions.Predictions.Count(x => x?.TenthPlacePoints == 25)}x25")
                        .AppendLine();
                }

                stringBuilder.Append("```");
                await RespondToInteractionAsync(interactionContext, stringBuilder.ToString());
            }
        );
    }

    private static int SumPoints(F1PredictionUserResultDto predictionUserResult)
    {
        return predictionUserResult.TenthPlacePoints
               + predictionUserResult.DnfsPoints
               + predictionUserResult.SafetyCarsPoints
               + predictionUserResult.FirstPlaceLeadPoints
               + predictionUserResult.TeamMatesPoints;
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    private readonly IInteractivityRepository interactivityRepository;
    private readonly IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder;
    private readonly IUsersCache usersCache;
}