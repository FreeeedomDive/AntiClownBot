using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Race;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

public class DriversCommand : ICommand
{
    public DriversCommand(IDiscordClientWrapper discordClientWrapper)
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
            .OrderByDescending(driver => driver.AccelerationStat + driver.BreakingStat + driver.CorneringStat)
            .Select(driver => (driver.Name, Math.Round(((driver.AccelerationStat + driver.BreakingStat + driver.CorneringStat) * 1000) / 3) / 10d))
            .ToDictionary(_ => position++, driver => driver).Select(kv => $"{kv.Key}. {kv.Value.Name} - {kv.Value.Item2}%");

        var orderedDriversString = string.Join("\n", orderedDriversDict);

        await discordClientWrapper.Messages.RespondAsync(e.Message, orderedDriversString);
    }

    public Task<string> Help()
    {
        return Task.FromResult("Текущий прогресс гонщиков\nПоказывает уровень прокачки умений каждого гонщика");
    }

    public string Name => "drivers";
    public bool IsObsolete => false;

    private readonly IDiscordClientWrapper discordClientWrapper;
}