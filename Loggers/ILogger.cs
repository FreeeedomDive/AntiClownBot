namespace Loggers;

public interface ILogger
{
    void Info(string message, params object[] args);
    void Error(string message, params object[] args);
    void Error(Exception exception, string message, params object?[] args);
}