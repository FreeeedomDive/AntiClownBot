﻿using AntiClown.DiscordBot.Cache.Users;
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
        await ExecuteAsync(
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
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Predict, "Добавить или изменить текущее предсказание конкретному челику")]
    public async Task Predict(
        InteractionContext interactionContext,
        [Option("user", "Челик, которому засчитать предсказание")]
        DiscordUser user,
        [Option("driver", "10 место в гонке")] F1DriverDto tenthPlaceDriver,
        [Option("dnf", "Первый DNF в гонке")] F1DriverDto dnfDriver
    )
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

                var userId = await usersCache.GetApiIdByMemberIdAsync(user.Id);
                await antiClownEntertainmentApiClient.F1Predictions.AddPredictionAsync(currentRace.Details!.RaceId, userId, tenthPlaceDriver, dnfDriver);
                await RespondToInteractionAsync(interactionContext, "Принято");
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Close, "Закрыть предсказания")]
    public async Task ClosePredictions(InteractionContext interactionContext)
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

                await antiClownEntertainmentApiClient.F1Predictions.ClosePredictionsAsync(currentRace.Details!.RaceId);
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Results, "Внести результаты гонки")]
    public async Task CreateRaceResultsMessage(InteractionContext interactionContext)
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
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Dnf, "Внести результаты первого выбывшего гонщика")]
    public async Task MakeFirstDnfResults(
        InteractionContext interactionContext,
        [Option("driver", "Первый выбывший гонщик")]
        F1DriverDto dnfDriver
    )
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

                currentRace.Details!.FirstDnf = dnfDriver;
                await RespondToInteractionAsync(interactionContext, "Принято");
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.F1Admin_Finish, "Завершить текущую гонку")]
    public async Task FinishCurrentRacePredictions(InteractionContext interactionContext)
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

                var raceDetails = currentRace.Details!;
                var raceId = currentRace.Details!.RaceId;
                if (raceDetails.Classification.Count != 20)
                {
                    await RespondToInteractionAsync(interactionContext, "В финальной таблице внесено менее 20 гонщиков");
                    return;
                }

                await antiClownEntertainmentApiClient.F1Predictions.AddClassificationsResultAsync(raceId, raceDetails.Classification.ToArray());
                if (raceDetails.FirstDnf is not null)
                {
                    await antiClownEntertainmentApiClient.F1Predictions.AddFirstDnfResultAsync(raceId, raceDetails.FirstDnf!.Value);
                }

                var raceResults = await antiClownEntertainmentApiClient.F1Predictions.FinishRaceAsync(raceId);
                var embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Результаты предсказаний");
                await raceResults.ForEachAsync(
                    async x =>
                    {
                        var member = await usersCache.GetMemberByApiIdAsync(x.UserId);
                        embedBuilder.AddField(
                            member.ServerOrUserName(),
                            $"{x.TenthPlacePoints.ToPluralizedString("очко", "очка", "очков")} очков за предсказание 10 места, "
                            + $"{x.FirstDnfPoints.ToPluralizedString("очко", "очка", "очков")} за предсказание первого DNF"
                        );
                    }
                );

                await RespondToInteractionAsync(interactionContext, embedBuilder.Build());
                currentRace.Type = InteractivityType.F1PredictionsFinished;
                await interactivityRepository.UpdateAsync(currentRace);
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IUsersCache usersCache;
}