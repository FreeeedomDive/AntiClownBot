using AntiClown.Core.Schedules;

namespace AntiClown.Api.Core.IntegrationTests.Mocks;

public class SchedulerMock : IScheduler
{
    public void Schedule(Action scheduleAction)
    {
        Scheduled = true;
    }

    public bool Scheduled { get; private set; }
}