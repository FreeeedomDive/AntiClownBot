namespace AntiClown.Entertainment.Api.Dto.CommonEvents;

public abstract class CommonEventBaseDto
{
    public Guid Id { get; set; }
    public DateTime EventDateTime { get; set; }
    public bool Finished { get; set; }
    public abstract CommonEventTypeDto Type { get; }
}