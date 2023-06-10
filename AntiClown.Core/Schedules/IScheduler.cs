namespace AntiClown.Core.Schedules;

public interface IScheduler
{
    void Schedule(Func<Task> scheduleAction, TimeSpan delay);    
    void Schedule(Action scheduleAction, TimeSpan delay);
}