using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.F1;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other.F1Predictions;

[SlashCommandGroup("f1", "Команды, связанные с гонками Ф1")]
public class F1CommandModule : ApplicationCommandModule
{
    public F1CommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IF1PredictionsService f1PredictionsService,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.f1PredictionsService = f1PredictionsService;
        this.guildSettingsService = guildSettingsService;
    }

    [SlashCommand("predict", "Добавить или изменить свое текущее предсказание")]
    public async Task Predict(
        InteractionContext interactionContext,
        [Option("tenthPlace", "10 место в гонке")]
        F1Driver tenthPlaceDriver,
        [Option("dnf", "Первый DNF в гонке")] F1Driver dnfDriver
    )
    {
        f1PredictionsService.PredictTenthPlace(interactionContext.Member.Id, tenthPlaceDriver);
        f1PredictionsService.PredictDnf(interactionContext.Member.Id, dnfDriver);
        await discordClientWrapper.Messages.RespondAsync(interactionContext, "Принято");
    }

    [SlashCommand("list", "Показать текущие предсказания")]
    public async Task ListPredictions(InteractionContext interactionContext)
    {
        var members = (await discordClientWrapper.Guilds.GetGuildAsync()).Members;
        var tenthPlacePredictions = f1PredictionsService.GetTenthPlacePredictions();
        var firstDnfPredictions = f1PredictionsService.GetFirstDnfPredictions();
        var embed = new DiscordEmbedBuilder()
            .AddField("10 место",
                string.Join(
                    "\n",
                    tenthPlacePredictions.Select(kv => $"{members[kv.Key].ServerOrUserName()}: {kv.Value}"))
            )
            .AddField("Первый DNF",
                string.Join(
                    "\n",
                    firstDnfPredictions.Select(kv => $"{members[kv.Key].ServerOrUserName()}: {kv.Value}"))
            )
            .Build();
        await discordClientWrapper.Messages.RespondAsync(interactionContext, embed);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IF1PredictionsService f1PredictionsService;
    private readonly IGuildSettingsService guildSettingsService;
}