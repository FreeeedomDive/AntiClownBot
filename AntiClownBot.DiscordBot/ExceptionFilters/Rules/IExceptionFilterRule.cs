namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public interface IExceptionFilterRule
{
    bool Filter(Exception exception);
}