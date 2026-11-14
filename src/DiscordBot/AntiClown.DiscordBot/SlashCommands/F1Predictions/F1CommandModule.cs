using System.Text;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.F1Predictions.Client;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1_Group, "Команды, связанные с гонками Ф1")]
public class F1CommandModule(
    ICommandExecutor commandExecutor,
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IF1PredictionsClient f1PredictionsClient,
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
                var selectedSeason = season is null ? DateTime.UtcNow.Year : (int)season;
                AntiClown.F1Predictions.Dto.SeasonStandingsDto standings;
                try
                {
                    standings = await f1PredictionsClient.GetStandingsAsync(selectedSeason);
                }
                catch (F1PredictionsException)
                {
                    await RespondToInteractionAsync(interactionContext, "Не удалось загрузить таблицу F1Predictions. Попробуйте позже.");
                    return;
                }
                if (standings.Rows.Length == 0 || standings.Events.Length == 0)
                {
                    await RespondToInteractionAsync(interactionContext, $"В сезоне {selectedSeason} еще не было ни одной гонки");
                    return;
                }

                var names = new Dictionary<Guid, string>();
                foreach (var row in standings.Rows)
                {
                    var member = await usersCache.GetMemberByApiIdAsync(row.ParticipantId);
                    var name = (member is null ? row.ParticipantName : member.ServerOrUserName())
                        .Replace('`', '\'').Replace('\n', ' ').Replace('\r', ' ');
                    names[row.ParticipantId] = name.Length > 32 ? name[..32] : name;
                }
                var longestNameLength = names.Values.Max(x => x.Length);
                var links = new StringBuilder($"Полная таблица: {webOptions.Value.FrontApplicationUrl}/f1Predictions/standings?season={selectedSeason}")
                    .Append($"\nРегламент: {webOptions.Value.FrontApplicationUrl}/f1Predictions/rulebook")
                    .ToString();
                var chunks = new List<string>();
                var stringBuilder = new StringBuilder(links).Append("\n```\n");
                var position = 1;
                foreach (var row in standings.Rows)
                {
                    var points = row.Results.ToDictionary(x => x.RaceId, x => x.Points);
                    var line = $"{position++} | {names[row.ParticipantId].AddSpaces(longestNameLength, false)} | "
                        + string.Join(" ", standings.Events.Select(race => points.GetValueOrDefault(race.Id) is { } result
                            ? result.TotalPoints.ToString().AddSpaces(2) : "  "))
                        + $" | {row.TotalPoints.AddSpaces(3)} | {row.MaximumTenthPlaceCount}x25\n";
                    if (stringBuilder.Length + line.Length + 3 > 1950)
                    {
                        chunks.Add(stringBuilder.Append("```").ToString());
                        stringBuilder = new StringBuilder("```\n");
                    }
                    stringBuilder.Append(line);
                }

                chunks.Add(stringBuilder.Append("```").ToString());
                await RespondToInteractionAsync(interactionContext, chunks[0]);
                foreach (var chunk in chunks.Skip(1))
                {
                    await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(chunk));
                }
            }
        );
    }
}
