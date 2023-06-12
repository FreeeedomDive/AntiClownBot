namespace AntiClown.EntertainmentApi.Dto.CommonEvents;

public class CommonEventBaseDto
{
    public Guid Id { get; set; }
    public DateTime EventDateTime { get; set; }
    public bool Finished { get; set; }
}