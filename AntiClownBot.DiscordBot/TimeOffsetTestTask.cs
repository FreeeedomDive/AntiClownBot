using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2;

public class TimeOffsetTestTask
{
    private readonly ILoggerClient log;

    public TimeOffsetTestTask(ILoggerClient log)
    {
        this.log = log;
    }

    public void Start()
    {
        Task.Run(StartAsync);
    }

    private async Task StartAsync()
    {
        while (true)
        {
            await log.DebugAsync("Current time is {time}, lets sleep for exactly 2 hours Bedge", DateTime.Now);
            await Task.Delay(2 * 60 * 60 * 1000);
        }
    }
}