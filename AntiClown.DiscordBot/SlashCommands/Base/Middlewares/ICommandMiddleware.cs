namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

public interface ICommandMiddleware
{
    Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next);
}