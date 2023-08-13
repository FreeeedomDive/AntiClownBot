using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Other.Race;

[SlashCommandGroup(InteractionsIds.CommandsNames.Race_Group, "Просмотр результатов гонок")]
public class RaceCommandModule : SlashCommandModuleWithMiddlewares
{
    public RaceCommandModule(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ICommandExecutor commandExecutor
    ) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Race_Drivers, "Посмотреть прокачку гонщиков")]
    public async Task DriversStats(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var drivers = await antiClownEntertainmentApiClient.CommonEvents.Race.Drivers.ReadAllAsync();
                var longestNameLength = drivers.MaxBy(x => x.DriverName.Length)!.DriverName.Length;
                var driversStats = drivers
                                   .OrderByDescending(x => x.AccelerationSkill + x.BreakingSkill + x.CorneringSkill)
                                   .Select(x => $"{x.DriverName.AddSpaces(longestNameLength)} | "
                                                + $"{x.AccelerationSkill:0.000} | "
                                                + $"{x.BreakingSkill:0.000} | "
                                                + $"{x.CorneringSkill:0.000}");
                var messageContent = string.Join("\n", driversStats);
                await RespondToInteractionAsync(interactionContext, $"```{messageContent}```");
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Race_Standings, "Посмотреть личный зачет")]
    public async Task DriversStandings(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext, async () =>
            {
                var drivers = await antiClownEntertainmentApiClient.CommonEvents.Race.Drivers.ReadAllAsync();
                var longestNameLength = drivers.MaxBy(x => x.DriverName.Length)!.DriverName.Length;
                var driversStats = drivers
                                   .OrderByDescending(x => x.Points)
                                   .Select(x => $"{x.DriverName.AddSpaces(longestNameLength)} | {x.Points}");
                var messageContent = string.Join("\n", driversStats);
                await RespondToInteractionAsync(interactionContext, $"```{messageContent}```");
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}