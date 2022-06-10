using System;
using AntiClownBot.Helpers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.Gaming;

public class PartyStatsCommand : BaseCommand
{
    public PartyStatsCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
    {
    }

    public override async void Execute(MessageCreateEventArgs e)
    {
        if (Config.PartyStats.TotalFullParties == 0)
        {
            await e.Message.RespondAsync("Не собрано ни одного пати для сбора статистики");
            return;
        }

        await e.Message
            .RespondAsync(
                $"Самое быстрое пати было собрано за {Utility.GetTimeDiff(TimeSpan.FromSeconds(Config.PartyStats.FastestPartyInSeconds))}\n" +
                $"В среднем пати собиралось за {Utility.GetTimeDiff(TimeSpan.FromSeconds(Config.PartyStats.TotalSeconds / Config.PartyStats.TotalFullParties))}");
    }

    public override string Help() => "Статистика по времени сбора фулл пати";
}