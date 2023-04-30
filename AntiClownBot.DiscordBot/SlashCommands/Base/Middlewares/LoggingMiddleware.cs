using System.Diagnostics;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.SlashCommands;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;

/// <summary>
///     Миддлварка собирает информацию об используемых командах и пишет логи об ошибках в телеметрию
/// </summary>
public class LoggingMiddleware : ICommandMiddleware
{
    public LoggingMiddleware(ILoggerClient loggerClient)
    {
        this.loggerClient = loggerClient;
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
            await loggerClient.ErrorAsync(e, "COMMAND {commandName}: Unhandled exception", commandName);
        }

        await loggerClient.InfoAsync(
            "COMMAND {commandName} executed by {userName} in {time}ms",
            commandName,
            context.Context.Member.ServerOrUserName(),
            stopwatch.ElapsedMilliseconds
        );
    }

    private readonly ILoggerClient loggerClient;
}