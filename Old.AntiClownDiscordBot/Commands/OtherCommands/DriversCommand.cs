using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AntiClownBot.Models.Race;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace AntiClownBot.Commands.OtherCommands;

public class DriversCommand: BaseCommand
{
    public DriversCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
    {
    }

    public override async void Execute(MessageCreateEventArgs e)
    {
        var driversContent = await File.ReadAllTextAsync("drivers.json");
        var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
        if (driversModels == null)
        {
            await e.Message.RespondAsync("");
            return;
        }

        var position = 1;
        var orderedDriversDict = driversModels
            .OrderByDescending(driver => driver.AccelerationStat + driver.BreakingStat + driver.CorneringStat)
            .Select(driver => (driver.Name, Math.Round(((driver.AccelerationStat + driver.BreakingStat + driver.CorneringStat) * 1000) / 3) / 10d))
            .ToDictionary(_ => position++, driver => driver).Select(kv => $"{kv.Key}. {kv.Value.Name} - {kv.Value.Item2}%");

        var orderedDriversString = string.Join("\n", orderedDriversDict);

        await e.Message.RespondAsync(orderedDriversString);
    }

    public override string Help()
    {
        return "Текущий прогресс гонщиков\nПоказывает уровень прокачки умений каждого гонщика";
    }
}