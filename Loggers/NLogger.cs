using NLog;

namespace Loggers;

public class NLogger : ILogger
{
    public NLogger(Logger logger)
    {
        this.logger = logger;
    }

    public void Info(string message, params object[] args)
    {
        logger.Info(message, args);
    }

    public void Error(string message, params object[] args)
    {
        logger.Error(message, args);
    }

    public void Error(string message, Exception exception)
    {
        logger.Error(message, exception);
    }

    private readonly Logger logger;
}