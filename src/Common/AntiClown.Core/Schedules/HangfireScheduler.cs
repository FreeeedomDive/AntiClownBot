namespace AntiClown.Core.Schedules;

public class HangfireScheduler : IScheduler
{
    public void Schedule(Action scheduleAction)
    {
        scheduleAction();
    }
}