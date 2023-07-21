using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.F1Predictions;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1_Group, "Команды, связанные с гонками Ф1")]
public class F1CommandModule : SlashCommandModuleWithMiddlewares
{
    public F1CommandModule(
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

    [SlashCommand(InteractionsIds.CommandsNames.F1_Predict, "Добавить или изменить свое текущее предсказание")]
    public async Task Predict(
        InteractionContext interactionContext,
        [Option("tenthPlace", "10 место в гонке")]
        F1DriverDto tenthPlaceDriver,
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

                var userId = await usersCache.GetApiIdByMemberIdAsync(interactionContext.Member.Id);
                await antiClownEntertainmentApiClient.F1Predictions.AddPredictionAsync(currentRace.Details!.RaceId, userId, tenthPlaceDriver, dnfDriver);
                await RespondToInteractionAsync(interactionContext, "Принято");
            }
        );
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

                var apiIdToMember = race.Predictions.ToDictionary(
                    x => x.UserId,
                    x => usersCache.GetMemberByApiIdAsync(x.UserId).GetAwaiter().GetResult()
                );
                var embed = new DiscordEmbedBuilder()
                            .AddField(
                                "10 место",
                                string.Join(
                                    "\n",
                                    race.Predictions.Select(p => $"{apiIdToMember[p.UserId].ServerOrUserName()}: {p.TenthPlacePickedDriver}")
                                )
                            )
                            .AddField(
                                "Первый DNF",
                                string.Join(
                                    "\n",
                                    race.Predictions.Select(p => $"{apiIdToMember[p.UserId].ServerOrUserName()}: {p.FirstDnfPickedDriver}")
                                )
                            ).Build();
                await RespondToInteractionAsync(interactionContext, embed);
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    private readonly IInteractivityRepository interactivityRepository;
    private readonly IUsersCache usersCache;
}