namespace AntiClown.EntertainmentApi.Dto.DailyEvents;

public abstract class DailyEventBaseDto
{
    public Guid Id { get; set; }
    public abstract DailyEventTypeDto Type { get; }
}