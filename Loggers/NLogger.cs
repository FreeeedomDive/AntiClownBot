using NLog;

namespace Loggers;

public class NLogger : ILogger
{
    private NLogger(Logger logger)
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

    public void Error(Exception exception, string message, params object?[] args)
    {
        logger.Error(exception, message, args);
    }

    public static NLogger Build(string name)
    {
        var logger = LogManager.GetLogger(name);
        return new NLogger(logger);
    }

    private readonly Logger logger;
}