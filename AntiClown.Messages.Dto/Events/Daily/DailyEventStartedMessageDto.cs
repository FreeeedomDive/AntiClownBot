namespace AntiClown.Messages.Dto.Events.Daily;

public class DailyEventStartedMessageDto
{
    public Guid EventId { get; set; }
    public DailyEventTypeDto EventType { get; set; }
}