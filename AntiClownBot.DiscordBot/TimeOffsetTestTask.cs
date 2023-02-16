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
            var now = DateTime.Now;
            await log.DebugAsync(
                "Current time is {time}, lets sleep for exactly 2 hours Bedge", 
                $"{now.Day}.{now.Month}.{now.Year} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}"
                );
            await Task.Delay(2 * 60 * 60 * 1000);
        }
    }
}