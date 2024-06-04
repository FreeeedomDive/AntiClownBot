using AntiClown.DiscordBot.EmbedBuilders.GuessNumber;
using AntiClown.DiscordBot.EmbedBuilders.Lottery;
using AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;
using AntiClown.DiscordBot.EmbedBuilders.Transfusion;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents;
using AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Other.Events;

[SlashCommandGroup(InteractionsIds.CommandsNames.Dev_EventsEditor_Group, "Управление эвентами", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
public class EventsCommandModule : SlashCommandModuleWithMiddlewares
{
    public EventsCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IGuessNumberEmbedBuilder guessNumberEmbedBuilder,
        ILotteryEmbedBuilder lotteryEmbedBuilder,
        ITransfusionEmbedBuilder transfusionEmbedBuilder,
        IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.guessNumberEmbedBuilder = guessNumberEmbedBuilder;
        this.lotteryEmbedBuilder = lotteryEmbedBuilder;
        this.transfusionEmbedBuilder = transfusionEmbedBuilder;
        this.removeCoolDownsEmbedBuilder = removeCoolDownsEmbedBuilder;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_EventsEditor_Read, "Посмотреть эвент")]
    public async Task ShowEventInfo(
        InteractionContext context,
        [Option("type", "Тип эвента")] CommonEventTypeDto commonEventTypeDto,
        [Option("id", "ID эвента")] string id
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                try
                {
                    var eventId = Guid.TryParse(id, out var x) ? x : throw new InvalidOperationException("Неверный формат гуида");
                    switch (commonEventTypeDto)
                    {
                        case CommonEventTypeDto.GuessNumber:
                            var guessNumberEvent = await antiClownEntertainmentApiClient.GuessNumberEvent.ReadAsync(eventId);
                            var guessNumberEmbed = await guessNumberEmbedBuilder.BuildAsync(guessNumberEvent);
                            await RespondToInteractionAsync(context, guessNumberEmbed);
                            break;
                        case CommonEventTypeDto.Lottery:
                            var lotteryEvent = await antiClownEntertainmentApiClient.LotteryEvent.ReadAsync(eventId);
                            var lotteryEmbed = await lotteryEmbedBuilder.BuildAsync(lotteryEvent);
                            await RespondToInteractionAsync(context, lotteryEmbed);
                            break;
                        case CommonEventTypeDto.Race:
                            throw new NotSupportedException("Отображение гонок пока недоступно");
                        case CommonEventTypeDto.RemoveCoolDowns:
                            var removeCoolDownsEvent = await antiClownEntertainmentApiClient.RemoveCoolDownsEvent.ReadAsync(eventId);
                            var removeCoolDownsEmbed = await removeCoolDownsEmbedBuilder.BuildAsync(removeCoolDownsEvent);
                            await RespondToInteractionAsync(context, removeCoolDownsEmbed);
                            break;
                        case CommonEventTypeDto.Transfusion:
                            var transfusionEvent = await antiClownEntertainmentApiClient.TransfusionEvent.ReadAsync(eventId);
                            var transfusionEmbed = await transfusionEmbedBuilder.BuildAsync(transfusionEvent);
                            await RespondToInteractionAsync(context, transfusionEmbed);
                            break;
                        case CommonEventTypeDto.Bedge:
                            throw new NotSupportedException("Отображение спящих эвентов не поддерживается");
                        default:
                            throw new ArgumentOutOfRangeException(nameof(commonEventTypeDto), commonEventTypeDto, null);
                    }
                }
                catch (Exception e)
                {
                    await RespondToInteractionAsync(context, e.Message);
                }
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_EventsEditor_Edit, "Включить или выключить эвент")]
    public async Task EditEventType(
        InteractionContext context,
        [Option("type", "Тип эвента")] CommonEventTypeDto commonEventTypeDto,
        [Option("operation", "Включить / выключить эвент")]
        EventOperation operation
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                await antiClownEntertainmentApiClient.ActiveEventsIndex.UpdateAsync(new ActiveCommonEventIndexDto
                {
                    EventType = commonEventTypeDto, 
                    IsActive = operation == EventOperation.Enable,
                });
                var allEventTypes = await antiClownEntertainmentApiClient.ActiveEventsIndex.ReadAllEventTypesAsync();
                var isEventPresent = allEventTypes.TryGetValue(commonEventTypeDto, out var isEnabled);
                var eventResult = isEventPresent ? isEnabled ? "enabled" : "disabled" : "not presented in list";
                await RespondToInteractionAsync(context, $"{commonEventTypeDto} is {eventResult}");
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_EventsEditor_Start, "Начать новый эвент")]
    public async Task StartEvent(
        InteractionContext context,
        [Option("type", "Тип эвента")] CommonEventTypeDto commonEventTypeDto
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var newEventId = commonEventTypeDto switch
                {
                    CommonEventTypeDto.GuessNumber => await antiClownEntertainmentApiClient.GuessNumberEvent.StartNewAsync(),
                    CommonEventTypeDto.Lottery => await antiClownEntertainmentApiClient.LotteryEvent.StartNewAsync(),
                    CommonEventTypeDto.Race => await antiClownEntertainmentApiClient.RaceEvent.StartNewAsync(),
                    CommonEventTypeDto.RemoveCoolDowns => await antiClownEntertainmentApiClient.RemoveCoolDownsEvent.StartNewAsync(),
                    CommonEventTypeDto.Transfusion => await antiClownEntertainmentApiClient.TransfusionEvent.StartNewAsync(),
                    CommonEventTypeDto.Bedge => await antiClownEntertainmentApiClient.BedgeEvent.StartNewAsync(),
                    _ => throw new ArgumentOutOfRangeException(nameof(commonEventTypeDto), commonEventTypeDto, null),
                };
                await RespondToInteractionAsync(context, newEventId.ToString());
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_EventsEditor_Finish, "Закончить эвент")]
    public async Task FinishEvent(
        InteractionContext context,
        [Option("type", "Тип эвента")] CommonEventTypeDto commonEventTypeDto,
        [Option("id", "ID эвента")] string id
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var eventId = Guid.TryParse(id, out var x) ? x : throw new InvalidOperationException("Неверный формат гуида");
                switch(commonEventTypeDto)
                {
                    case CommonEventTypeDto.GuessNumber:
                        await antiClownEntertainmentApiClient.GuessNumberEvent.FinishAsync(eventId);
                        break;
                    case CommonEventTypeDto.Lottery: 
                        await antiClownEntertainmentApiClient.LotteryEvent.FinishAsync(eventId);
                        break;
                    case CommonEventTypeDto.Race: 
                        await antiClownEntertainmentApiClient.RaceEvent.FinishAsync(eventId);
                        break;
                }
                await RespondToInteractionAsync(context, $"Event {eventId} is finished");
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IGuessNumberEmbedBuilder guessNumberEmbedBuilder;
    private readonly ILotteryEmbedBuilder lotteryEmbedBuilder;
    private readonly IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder;
    private readonly ITransfusionEmbedBuilder transfusionEmbedBuilder;
}