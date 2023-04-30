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
        [Option("driver", "10 место в гонке")]
        F1Driver tenthPlaceDriver,
        [Option("dnf", "Первый DNF в гонке")] F1Driver dnfDriver
    )
    {
        f1PredictionsService.PredictTenthPlace(user.Id, tenthPlaceDriver);
        f1PredictionsService.PredictDnf(user.Id, dnfDriver);
        await discordClientWrapper.Messages.RespondAsync(interactionContext, "Принято");
    }

    [SlashCommand("race", "Внести результаты гонки")]
    public async Task MakeTenthPlaceResults(
        InteractionContext interactionContext,
        [Option("p1", "1 место")] F1Driver p1,
        [Option("p2", "2 место")] F1Driver p2,
        [Option("p3", "3 место")] F1Driver p3,
        [Option("p4", "4 место")] F1Driver p4,
        [Option("p5", "5 место")] F1Driver p5,
        [Option("p6", "6 место")] F1Driver p6,
        [Option("p7", "7 место")] F1Driver p7,
        [Option("p8", "8 место")] F1Driver p8,
        [Option("p9", "9 место")] F1Driver p9,
        [Option("p10", "10 место")] F1Driver p10,
        [Option("p11", "11 место")] F1Driver p11,
        [Option("p12", "12 место")] F1Driver p12,
        [Option("p13", "13 место")] F1Driver p13,
        [Option("p14", "14 место")] F1Driver p14,
        [Option("p15", "15 место")] F1Driver p15,
        [Option("p16", "16 место")] F1Driver p16,
        [Option("p17", "17 место")] F1Driver p17,
        [Option("p18", "18 место")] F1Driver p18,
        [Option("p19", "19 место")] F1Driver p19,
        [Option("p20", "20 место")] F1Driver p20
    )
    {
        var results = f1PredictionsService.MakeTenthPlaceResults(
            p1,
            p2,
            p3,
            p4,
            p5,
            p6,
            p7,
            p8,
            p9,
            p10,
            p11,
            p12,
            p13,
            p14,
            p15,
            p16,
            p17,
            p18,
            p19,
            p20
        );
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