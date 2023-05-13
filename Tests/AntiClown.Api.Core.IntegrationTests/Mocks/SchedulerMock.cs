using AntiClown.Api.Core.Schedules;

namespace AntiClown.Api.Core.IntegrationTests.Mocks;

public class SchedulerMock : IScheduler
{
    public void Schedule(Func<Task> scheduleAction, TimeSpan delay)
    {
        Scheduled = true;
    }

    public void Schedule(Action scheduleAction, TimeSpan delay)
    {
        Scheduled = true;
    }

    public bool Scheduled { get; private set; }
}