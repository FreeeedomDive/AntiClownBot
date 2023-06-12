namespace AntiClown.EntertainmentApi.Dto.CommonEvents.GuessNumber;

public class GuessNumberUserPickDto
{
    public Guid UserId { get; set; }
    public GuessNumberPickDto Pick { get; set; }
}