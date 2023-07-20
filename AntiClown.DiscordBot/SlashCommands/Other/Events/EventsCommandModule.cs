using AntiClown.DiscordBot.EmbedBuilders.GuessNumber;
using AntiClown.DiscordBot.EmbedBuilders.Lottery;
using AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;
using AntiClown.DiscordBot.EmbedBuilders.Transfusion;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents;
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
                            var guessNumberEvent = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.ReadAsync(eventId);
                            var guessNumberEmbed = await guessNumberEmbedBuilder.BuildAsync(guessNumberEvent);
                            await RespondToInteractionAsync(context, guessNumberEmbed);
                            break;
                        case CommonEventTypeDto.Lottery:
                            var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
                            var lotteryEmbed = await lotteryEmbedBuilder.BuildAsync(lotteryEvent);
                            await RespondToInteractionAsync(context, lotteryEmbed);
                            break;
                        case CommonEventTypeDto.Race:
                            // var raceEvent = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(eventId);
                            // var raceEmbed = await raceEmbedBuilder.BuildAsync(raceEvent);
                            // await RespondToInteractionAsync(context, raceEmbed);
                            throw new NotSupportedException("Отображение гонок пока недоступно");
                            break;
                        case CommonEventTypeDto.RemoveCoolDowns:
                            var removeCoolDownsEvent = await antiClownEntertainmentApiClient.CommonEvents.RemoveCoolDowns.ReadAsync(eventId);
                            var removeCoolDownsEmbed = await removeCoolDownsEmbedBuilder.BuildAsync(removeCoolDownsEvent);
                            await RespondToInteractionAsync(context, removeCoolDownsEmbed);
                            break;
                        case CommonEventTypeDto.Transfusion:
                            var transfusionEvent = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.ReadAsync(eventId);
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
                await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.UpdateAsync(commonEventTypeDto, operation == EventOperation.Enable);
                var allEventTypes = await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadAllEventTypesAsync();
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
                    CommonEventTypeDto.GuessNumber => await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.StartNewAsync(),
                    CommonEventTypeDto.Lottery => await antiClownEntertainmentApiClient.CommonEvents.Lottery.StartNewAsync(),
                    CommonEventTypeDto.Race => await antiClownEntertainmentApiClient.CommonEvents.Race.StartNewAsync(),
                    CommonEventTypeDto.RemoveCoolDowns => await antiClownEntertainmentApiClient.CommonEvents.RemoveCoolDowns.StartNewAsync(),
                    CommonEventTypeDto.Transfusion => await antiClownEntertainmentApiClient.CommonEvents.Transfusion.StartNewAsync(),
                    CommonEventTypeDto.Bedge => await antiClownEntertainmentApiClient.CommonEvents.Bedge.StartNewAsync(),
                    _ => throw new ArgumentOutOfRangeException(nameof(commonEventTypeDto), commonEventTypeDto, null),
                };
                await RespondToInteractionAsync(context, newEventId.ToString());
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IGuessNumberEmbedBuilder guessNumberEmbedBuilder;
    private readonly ILotteryEmbedBuilder lotteryEmbedBuilder;
    private readonly IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder;
    private readonly ITransfusionEmbedBuilder transfusionEmbedBuilder;
}