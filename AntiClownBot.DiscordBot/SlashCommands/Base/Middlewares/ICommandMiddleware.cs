namespace AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;

public interface ICommandMiddleware
{
    Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next);
}