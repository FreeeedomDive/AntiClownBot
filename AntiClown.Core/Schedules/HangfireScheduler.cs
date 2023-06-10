using Hangfire;

namespace AntiClown.Core.Schedules;

public class HangfireScheduler : IScheduler
{
    public void Schedule(Func<Task> scheduleAction, TimeSpan delay)
    {
        BackgroundJob.Schedule(() => scheduleAction(), delay);
    }

    public void Schedule(Action scheduleAction, TimeSpan delay)
    {
        BackgroundJob.Schedule(() => scheduleAction(), delay);
    }
}