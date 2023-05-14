namespace AntiClown.Messages.Dto.Events.Common;

public class CommonEventFinishedMessageDto
{
    public Guid EventId { get; set; }
    public CommonEventTypeDto EventType { get; set; }
}