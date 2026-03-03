namespace AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

public class RaceParticipantDto
{
    public RaceDriverDto Driver { get; set; }
    public Guid? UserId { get; set; }
}