using System.Text;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1_Group, "Команды, связанные с гонками Ф1")]
public class F1CommandModule(
    ICommandExecutor commandExecutor,
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IUsersCache usersCache,
    IOptions<WebOptions> webOptions
) : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.F1_List, "Показать текущие предсказания")]
    public async Task ListPredictions(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var races = await antiClownEntertainmentApiClient.F1Predictions.FindAsync(
                    new F1RaceFilterDto
                    {
                        IsActive = true,
                    }
                );
                if (races.Length == 0)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                var embedBuilder = new DiscordEmbedBuilder();
                foreach (var race in races)
                {
                    var apiIdToMember = race.Predictions.ToDictionary(
                        x => x.UserId,
                        x => usersCache.GetMemberByApiIdAsync(x.UserId).GetAwaiter().GetResult()
                    );
                    embedBuilder.AddField(
                        $"Участники предсказания в гонке {race.Name} {race.Season}",
                        race.Predictions.Count == 0
                            ? "Еще никто не делал предсказания на эту гонку"
                            : string.Join("\n", race.Predictions.Select(x => apiIdToMember[x.UserId].ServerOrUserName()))
                    );
                }

                await RespondToInteractionAsync(interactionContext, embedBuilder.Build());
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
                var stringBuilder = new StringBuilder($"Полная таблица: {webOptions.Value.FrontApplicationUrl}/f1Predictions```\n");
                var predictionsTable = standings
                                       .Select(
                                           kv => new
                                           {
                                               UserId = kv.Key,
                                               Predictions = kv.Value,
                                               TotalPoints = kv.Value.Select(
                                                   p => p?.TotalPoints ?? 0
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
                                     .Select(p => p is null ? "  " : p.TotalPoints.ToString().AddSpaces(2))
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
}