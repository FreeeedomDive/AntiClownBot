namespace AntiClownDiscordBotVersion2.ExceptionFilters;

public interface IExceptionFilter
{
    bool Filter(Exception exception);
}