using System.Diagnostics;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Telemetry;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

/// <summary>
///     Миддлварка собирает информацию об используемых командах и пишет логи об ошибках в телеметрию
/// </summary>
public class LoggingMiddleware : ICommandMiddleware
{
    public LoggingMiddleware(ILogger<LoggingMiddleware> logger, DiscordTelemetry telemetry)
    {
        this.logger = logger;
        this.telemetry = telemetry;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        var commandName = context.Context.QualifiedName;
        var outcome = DiscordTelemetryValues.CommandOutcomes.Ok;
        var startedAt = Stopwatch.GetTimestamp();
        try
        {
            await next(context);
            if (context.IsRejected)
            {
                outcome = DiscordTelemetryValues.CommandOutcomes.Rejected;
            }
        }
        catch (Exception e)
        {
            outcome = DiscordTelemetryValues.CommandOutcomes.Error;
            Activity.Current?.SetStatus(ActivityStatusCode.Error, e.Message);
            Activity.Current?.AddException(e);
            logger.LogError(e, "COMMAND {commandName}: Unhandled exception", commandName);
        }

        var elapsed = Stopwatch.GetElapsedTime(startedAt);
        telemetry.RecordCommand(commandName, outcome, elapsed);

        logger.LogInformation(
            "COMMAND {commandName} executed by {userName} in {time}ms",
            commandName,
            context.Context.Member.ServerOrUserName(),
            elapsed.TotalMilliseconds
        );
    }

    private readonly ILogger<LoggingMiddleware> logger;
    private readonly DiscordTelemetry telemetry;
}
