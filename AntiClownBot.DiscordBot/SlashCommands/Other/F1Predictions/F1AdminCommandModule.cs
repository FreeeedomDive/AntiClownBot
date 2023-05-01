using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.F1;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other.F1Predictions;

[SlashCommandGroup("f1admin", "Админка для предиктов ф1", false)]
[SlashCommandPermissions(Permissions.ViewAuditLog)]
public class F1AdminCommandModule : SlashCommandModuleWithMiddlewares
{
    public F1AdminCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IF1PredictionsService f1PredictionsService
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.f1PredictionsService = f1PredictionsService;
    }

    [SlashCommand("predict", "Добавить или изменить текущее предсказание конкретному челику")]
    public async Task Predict(
        InteractionContext interactionContext,
        [Option("user", "Челик, которому засчитать предсказание")]
        DiscordUser user,
        [Option("driver", "10 место в гонке")] F1Driver tenthPlaceDriver,
        [Option("dnf", "Первый DNF в гонке")] F1Driver dnfDriver
    )
    {
        await ExecuteAsync(interactionContext, async () =>
        {
            f1PredictionsService.PredictTenthPlace(user.Id, tenthPlaceDriver);
            f1PredictionsService.PredictDnf(user.Id, dnfDriver);
            await RespondToInteractionAsync(interactionContext, "Принято");
        });
    }

    [SlashCommand("results", "Внести результаты гонки")]
    public async Task CreateRaceResultsMessage(InteractionContext interactionContext)
    {
        await ExecuteAsync(interactionContext, async () =>
        {
            var builder = new DiscordWebhookBuilder()
                .WithContent("Начать заполнение результатов гонки...")
                .AddComponents(
                    new DiscordButtonComponent(
                        ButtonStyle.Secondary,
                        "start_race_result_input",
                        "Начать..."
                    )
                );
            await RespondToInteractionAsync(interactionContext, builder);
        });
    }

    [SlashCommand("dnf", "Внести результаты первого выбывшего гонщика")]
    public async Task MakeTenthPlaceResults(
        InteractionContext interactionContext,
        [Option("driver", "Первый выбывший гонщик")]
        F1Driver dnfDriver
    )
    {
        await ExecuteAsync(interactionContext, async () =>
        {
            var results = f1PredictionsService.MakeFirstDnfResults(dnfDriver);
            if (results == null || results.Length == 0)
            {
                await RespondToInteractionAsync(interactionContext, "Никто не угадал");
                return;
            }

            var members = (await discordClientWrapper.Guilds.GetGuildAsync()).Members;
            var resultsStrings =
                results.Select(tuple => $"{members[tuple.userId].ServerOrUserName()}: {tuple.tenthPlacePoints}");
            await RespondToInteractionAsync(interactionContext, string.Join("\n", resultsStrings));
        });
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IF1PredictionsService f1PredictionsService;
}