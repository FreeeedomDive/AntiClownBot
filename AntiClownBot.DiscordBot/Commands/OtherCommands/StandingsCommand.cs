using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Race;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

[ObsoleteCommand("standings")]
public class StandingsCommand : ICommand
{
    public StandingsCommand(IDiscordClientWrapper discordClientWrapper)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");
        var driversContent = await File.ReadAllTextAsync($"{filesDirectory}/StatisticsFiles/drivers.json");
        var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
        if (driversModels == null)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "huh?");
            return;
        }

        var position = 1;
        var orderedDriversDict = driversModels
            .OrderByDescending(driver => driver.Points)
            .ToDictionary(_ => position++, driver => driver).Select(kv =>
            {
                var correctedPosition = kv.Key < 10 ? $" {kv.Key}": $"{kv.Key}";
                return $"{correctedPosition}. {kv.Value.ShortName} - {kv.Value.Points}";
            });

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