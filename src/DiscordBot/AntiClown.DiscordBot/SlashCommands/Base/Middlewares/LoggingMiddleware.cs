using System.Diagnostics;
using AntiClown.DiscordBot.Extensions;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

/// <summary>
///     Миддлварка собирает информацию об используемых командах и пишет логи об ошибках в телеметрию
/// </summary>
public class LoggingMiddleware : ICommandMiddleware
{
    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
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
            logger.LogError(e, "COMMAND {commandName}: Unhandled exception", commandName);
        }

        logger.LogInformation(
            "COMMAND {commandName} executed by {userName} in {time}ms",
            commandName,
            context.Context.Member.ServerOrUserName(),
            stopwatch.ElapsedMilliseconds
        );
    }

    private readonly ILogger<LoggingMiddleware> logger;
}