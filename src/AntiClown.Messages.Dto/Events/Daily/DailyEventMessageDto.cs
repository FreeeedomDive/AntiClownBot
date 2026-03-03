namespace AntiClown.Messages.Dto.Events.Daily;

public class DailyEventMessageDto
{
    public Guid EventId { get; set; }
    public DailyEventTypeDto EventType { get; set; }
}