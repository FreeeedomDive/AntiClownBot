using AntiClown.Data.Api.Dto.Rights;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.F1Predictions;

[SlashCommandGroup(InteractionsIds.CommandsNames.F1Admin_Group, "Админка для предиктов ф1", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
public class F1AdminCommandModule : SlashCommandModuleWithMiddlewares
{
    public F1AdminCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
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
                await antiClownEntertainmentApiClient.F1Predictions.StartNewRaceAsync(trackName);
                await RespondToInteractionAsync(interactionContext, $"Начались сборы предсказаний на гонку {trackName}");
            }, RightsDto.F1PredictionsAdmin
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}