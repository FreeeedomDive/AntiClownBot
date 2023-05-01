using AntiClownDiscordBotVersion2.Models.Race;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

[SlashCommandGroup("race", "Статистика гонок")]
public class RaceCommandModule : SlashCommandModuleWithMiddlewares
{
    public RaceCommandModule(ICommandExecutor commandExecutor) : base(commandExecutor)
    {
    }

    [SlashCommand("drivers", "Уровень прокачки гонщиков")]
    public async Task Drivers(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ??
                                 throw new Exception("AntiClownBotFilesDirectory env variable was null");
            var driversContent = await File.ReadAllTextAsync($"{filesDirectory}/StatisticsFiles/drivers.json");
            var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
            if (driversModels == null)
            {
                await RespondToInteractionAsync(context, "huh?");
                return;
            }

            var position = 1;
            var orderedDriversDict = driversModels
                .OrderByDescending(driver => driver.AccelerationStat + driver.BreakingStat + driver.CorneringStat)
                .Select(driver => (Name: driver.Name,
                    Stats: Math.Round(((driver.AccelerationStat + driver.BreakingStat + driver.CorneringStat) * 1000) / 3) /
                           10d))
                .ToDictionary(_ => position++, driver => driver)
                .Select(kv => $"{kv.Key}. {kv.Value.Name} - {kv.Value.Stats}%");

            var orderedDriversString = string.Join("\n", orderedDriversDict);

            await RespondToInteractionAsync(context, orderedDriversString);
        });
    }

    [SlashCommand("standings", "Личный зачет")]
    public async Task Standings(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ??
                                 throw new Exception("AntiClownBotFilesDirectory env variable was null");
            var driversContent = await File.ReadAllTextAsync($"{filesDirectory}/StatisticsFiles/drivers.json");
            var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
            if (driversModels == null)
            {
                await RespondToInteractionAsync(context, "huh?");
                return;
            }

            var position = 1;
            var orderedDriversDict = driversModels
                .OrderByDescending(driver => driver.Points)
                .ToDictionary(_ => position++, driver => driver).Select(kv =>
                {
                    var correctedPosition = kv.Key < 10 ? $" {kv.Key}" : $"{kv.Key}";
                    return $"{correctedPosition}. {kv.Value.ShortName} - {kv.Value.Points}";
                });

            var orderedDriversString = string.Join("\n", orderedDriversDict);

            await RespondToInteractionAsync(context, $"```{orderedDriversString}```");
        });
    }
}