using AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

namespace AntiClownDiscordBotVersion2.ExceptionFilters;

public class ExceptionFilter : IExceptionFilter
{
    public ExceptionFilter(IEnumerable<IExceptionFilterRule> rules)
    {
        this.rules = rules;
    }

    public bool Filter(Exception exception)
    {
        return rules.Any(rule => rule.Filter(exception));
    }

    private readonly IEnumerable<IExceptionFilterRule> rules;
}