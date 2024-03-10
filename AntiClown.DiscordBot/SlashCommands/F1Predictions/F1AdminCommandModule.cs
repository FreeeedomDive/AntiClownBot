using AntiClown.Data.Api.Dto.Rights;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.F1Predictions;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1Admin_Group, "Админка для предиктов ф1", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
public class F1AdminCommandModule : SlashCommandModuleWithMiddlewares
{
    public F1AdminCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IInteractivityRepository interactivityRepository,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.interactivityRepository = interactivityRepository;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Start, "Начать предсказания на новую гонку")]
    public async Task StartPredictions(
        InteractionContext interactionContext,
        [Option("trackname", "Название гонки")]
        string trackName
    )
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is not null)
                {
                    var currentRacePredictions = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(currentRace.Details!.RaceId);
                    await RespondToInteractionAsync(interactionContext, $"На данный момент уже запущены предсказания на гонку {currentRacePredictions.Name}");
                    return;
                }

                var newRaceId = await antiClownEntertainmentApiClient.F1Predictions.StartNewRaceAsync(trackName);
                var newRaceInteraction = new Interactivity<F1PredictionDetails>
                {
                    Id = newRaceId,
                    Type = InteractivityType.F1Predictions,
                    AuthorId = interactionContext.Member.Id,
                    CreatedAt = DateTime.UtcNow,
                    MessageId = 0,
                    Details = new F1PredictionDetails
                    {
                        RaceId = newRaceId,
                        Classification = new List<F1DriverDto>(),
                        FirstDnf = null,
                    },
                };
                await interactivityRepository.CreateAsync(newRaceInteraction);

                await RespondToInteractionAsync(interactionContext, $"Начались сборы предсказаний на гонку {trackName}");
            }, RightsDto.F1PredictionsAdmin
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Close, "Закрыть предсказания")]
    public async Task ClosePredictions(InteractionContext interactionContext)
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                await antiClownEntertainmentApiClient.F1Predictions.ClosePredictionsAsync(currentRace.Details!.RaceId);
                await RespondToInteractionAsync(interactionContext, "Предсказания закрыты");
            }, RightsDto.F1PredictionsAdmin
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Results, "Внести результаты гонки")]
    public async Task CreateRaceResultsMessage(InteractionContext interactionContext)
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                var builder = new DiscordWebhookBuilder()
                              .WithContent("Начать заполнение результатов гонки...")
                              .AddComponents(
                                  new DiscordButtonComponent(
                                      ButtonStyle.Secondary,
                                      InteractionsIds.F1PredictionsButtons.StartRaceResultInputButton,
                                      "Начать..."
                                  )
                              );
                var message = await RespondToInteractionAsync(interactionContext, builder);
                currentRace.Details!.Classification = new List<F1DriverDto>();
                currentRace.MessageId = message.Id;
                await interactivityRepository.UpdateAsync(currentRace);
            }, RightsDto.F1PredictionsAdmin
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Dnf, "Добавить выбывшего гонщика")]
    public async Task AddDnf(
        InteractionContext interactionContext,
        [Option("driver", "Выбывший гонщик")]
        F1DriverDto dnfDriver
    )
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                await antiClownEntertainmentApiClient.F1Predictions.AddDnfDriverAsync(currentRace.Id, dnfDriver);
                await RespondToInteractionAsync(interactionContext, "Принято");
            }, RightsDto.F1PredictionsAdmin
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Sc, "Добавить машину безопасности")]
    public async Task AddSc(
        InteractionContext interactionContext
    )
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                await antiClownEntertainmentApiClient.F1Predictions.AddSafetyCarAsync(currentRace.Id);
                await RespondToInteractionAsync(interactionContext, "Принято");
            }, RightsDto.F1PredictionsAdmin
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_FirstPlaceLead, "Добавить результат лидирования победителя")]
    public async Task AddFirstPlaceLead(
        InteractionContext interactionContext,
        [Option("lead", "Лидирование в формате x.xxx")] string lead
    )
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                if (!decimal.TryParse(lead, out var decimalLead))
                {
                    await RespondToInteractionAsync(interactionContext, "Неверный формат");
                    return;
                }
                await antiClownEntertainmentApiClient.F1Predictions.AddFirstPlaceLeadAsync(currentRace.Id, decimalLead);
                await RespondToInteractionAsync(interactionContext, "Принято");
            }, RightsDto.F1PredictionsAdmin
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Finish, "Завершить текущую гонку")]
    public async Task FinishCurrentRacePredictions(InteractionContext interactionContext)
    {
        await ExecuteWithRightsAsync(
            interactionContext, async () =>
            {
                var currentRace = (await interactivityRepository.FindByTypeAsync<F1PredictionDetails>(InteractivityType.F1Predictions)).FirstOrDefault();
                if (currentRace is null)
                {
                    await RespondToInteractionAsync(interactionContext, "На данный момент нет активных предсказаний на гонку");
                    return;
                }

                var raceId = currentRace.Details!.RaceId;

                var raceResults = await antiClownEntertainmentApiClient.F1Predictions.FinishAsync(raceId);
                var embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Результаты предсказаний");
                await raceResults.ForEachAsync(
                    async x =>
                    {
                        var member = await usersCache.GetMemberByApiIdAsync(x.UserId);
                        embedBuilder.AddField(
                            member.ServerOrUserName(),
                            $"{x.TenthPlacePoints.ToPluralizedString("очко", "очка", "очков")} за 10 место\n"
                            + $"{x.DnfsPoints.ToPluralizedString("очко", "очка", "очков")} за DNF\n"
                            + $"{x.SafetyCarsPoints.ToPluralizedString("очко", "очка", "очков")} за SC\n"
                            + $"{x.FirstPlaceLeadPoints.ToPluralizedString("очко", "очка", "очков")} за отрыв лидера\n"
                            + $"{x.TeamMatesPoints.ToPluralizedString("очко", "очка", "очков")} за победителей внутри команд\n"
                        );
                    }
                );

                await RespondToInteractionAsync(interactionContext, embedBuilder.Build());
                currentRace.Type = InteractivityType.F1PredictionsFinished;
                await interactivityRepository.UpdateAsync(currentRace);
            }, RightsDto.F1PredictionsAdmin
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IUsersCache usersCache;
}