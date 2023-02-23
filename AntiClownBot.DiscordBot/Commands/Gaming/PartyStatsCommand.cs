using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.Gaming;

[ObsoleteCommand("party -s")]
public class PartyStatsCommand : ICommand
{
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IPartyService partyService;

    public PartyStatsCommand(
        IDiscordClientWrapper discordClientWrapper,
        IPartyService partyService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.partyService = partyService;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        var partyStats = partyService.PartiesInfo.PartyStats;
        if (partyStats.TotalFullParties == 0)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "Не собрано ни одного пати для сбора статистики");
            return;
        }

        var content = $"Самое быстрое пати было собрано за {Utility.GetTimeDiff(TimeSpan.FromSeconds(partyStats.FastestPartyInSeconds))}\n" +
                      $"В среднем пати собиралось за {Utility.GetTimeDiff(TimeSpan.FromSeconds(partyStats.TotalSeconds / partyStats.TotalFullParties))}";
        await discordClientWrapper.Messages.RespondAsync(e.Message, content);
    }

    public Task<string> Help() => Task.FromResult("Статистика по времени сбора фулл пати");

    public string Name => "partystats";
}