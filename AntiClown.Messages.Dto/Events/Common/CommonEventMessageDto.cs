namespace AntiClown.Messages.Dto.Events.Common;

public class CommonEventMessageDto
{
    public Guid EventId { get; set; }
    public CommonEventTypeDto EventType { get; set; }
    public bool Finished { get; set; }
}