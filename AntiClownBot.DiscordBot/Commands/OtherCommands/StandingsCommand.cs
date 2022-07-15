using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Race;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

public class StandingsCommand : ICommand
{
    public StandingsCommand(IDiscordClientWrapper discordClientWrapper)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        var driversContent = await File.ReadAllTextAsync("../Files/StatisticsFiles/drivers.json");
        var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
        if (driversModels == null)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "huh?");
            return;
        }

        var position = 1;
        var orderedDriversDict = driversModels
            .OrderByDescending(driver => driver.Points)
            .ToDictionary(_ => position++, driver => driver).Select(kv => $"{kv.Key}. {kv.Value.ShortName} - {kv.Value.Points}");

        var orderedDriversString = string.Join("\n", orderedDriversDict);

        await discordClientWrapper.Messages.RespondAsync(e.Message, $"```{orderedDriversString}```");
    }

    public Task<string> Help()
    {
        return Task.FromResult("Положение гонщиков в чемпионате");
    }

    public string Name => "standings";

    private readonly IDiscordClientWrapper discordClientWrapper;
}