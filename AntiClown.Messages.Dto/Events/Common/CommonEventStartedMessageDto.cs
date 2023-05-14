namespace AntiClown.Messages.Dto.Events.Common;

public class CommonEventStartedMessageDto
{
    public Guid EventId { get; set; }
    public CommonEventTypeDto EventType { get; set; }
}