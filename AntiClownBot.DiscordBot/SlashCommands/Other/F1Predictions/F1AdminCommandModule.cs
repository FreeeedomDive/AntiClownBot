using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.F1;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other.F1Predictions;

[SlashCommandGroup("f1admin", "Админка для предиктов ф1", false)]
[SlashCommandPermissions(Permissions.ViewAuditLog)]
public class F1AdminCommandModule : ApplicationCommandModule
{
    public F1AdminCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IF1PredictionsService f1PredictionsService
    )
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
        f1PredictionsService.PredictTenthPlace(user.Id, tenthPlaceDriver);
        f1PredictionsService.PredictDnf(user.Id, dnfDriver);
        await discordClientWrapper.Messages.RespondAsync(interactionContext, "Принято");
    }

    [SlashCommand("results", "Внести результаты гонки")]
    public async Task CreateRaceResultsMessage(InteractionContext interactionContext)
    {
        var drivers = f1PredictionsService.DriversToAddToResult();
        if (drivers.Length == 0)
        {
            await discordClientWrapper.Messages.RespondAsync(
                interactionContext,
                "Все гонщики внесены, можно подводить результаты"
            );
        }

        var options = drivers.Select(driver => new DiscordSelectComponentOption(
            driver.ToString(),
            $"driver_select_{driver.ToString().ToLower()}"
        ));
        var currentPlaceToEnter = 20 - drivers.Length + 1;
        var dropdown = new DiscordSelectComponent("dropdown", $"Гонщик на {currentPlaceToEnter} месте", options);
        var builder = new DiscordInteractionResponseBuilder()
            .WithContent($"Результаты гонки, {currentPlaceToEnter} место")
            .AddComponents(dropdown);

        await discordClientWrapper.Messages.RespondAsync(interactionContext.Interaction, InteractionResponseType.ChannelMessageWithSource ,builder);
    }

    [SlashCommand("race", "Внести результаты гонки")]
    public async Task MakeTenthPlaceResults(InteractionContext interactionContext)
    {
        var results = f1PredictionsService.MakeTenthPlaceResults();
        var members = (await discordClientWrapper.Guilds.GetGuildAsync()).Members;
        var resultsStrings =
            results.Select(tuple => $"{members[tuple.userId].ServerOrUserName()}: {tuple.tenthPlacePoints}");
        await discordClientWrapper.Messages.RespondAsync(interactionContext, string.Join("\n", resultsStrings));
    }

    [SlashCommand("dnf", "Внести результаты первого выбывшего гонщика")]
    public async Task MakeTenthPlaceResults(
        InteractionContext interactionContext,
        [Option("driver", "Первый выбывший гонщик")]
        F1Driver dnfDriver
    )
    {
        var results = f1PredictionsService.MakeFirstDnfResults(dnfDriver);
        if (results == null || results.Length == 0)
        {
            await discordClientWrapper.Messages.RespondAsync(interactionContext, "Никто не угадал");
            return;
        }

        var members = (await discordClientWrapper.Guilds.GetGuildAsync()).Members;
        var resultsStrings =
            results.Select(tuple => $"{members[tuple.userId].ServerOrUserName()}: {tuple.tenthPlacePoints}");
        await discordClientWrapper.Messages.RespondAsync(interactionContext, string.Join("\n", resultsStrings));
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IF1PredictionsService f1PredictionsService;
}