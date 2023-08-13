namespace AntiClown.Core.Schedules;

public interface IScheduler
{
    void Schedule(Action scheduleAction);
}