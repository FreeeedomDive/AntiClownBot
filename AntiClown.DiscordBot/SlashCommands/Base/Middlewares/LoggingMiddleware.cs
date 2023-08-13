using System.Diagnostics;
using AntiClown.DiscordBot.Extensions;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

/// <summary>
///     Миддлварка собирает информацию об используемых командах и пишет логи об ошибках в телеметрию
/// </summary>
public class LoggingMiddleware : ICommandMiddleware
{
    public LoggingMiddleware(ILoggerClient logger)
    {
        this.logger = logger;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        var commandName = context.Context.CommandName;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e, "COMMAND {commandName}: Unhandled exception", commandName);
        }

        await logger.InfoAsync(
            "COMMAND {commandName} executed by {userName} in {time}ms",
            commandName,
            context.Context.Member.ServerOrUserName(),
            stopwatch.ElapsedMilliseconds
        );
    }

    private readonly ILoggerClient logger;
}