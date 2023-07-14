namespace AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;

public class LotteryEventDto : CommonEventBaseDto
{
    public override CommonEventTypeDto Type => CommonEventTypeDto.Lottery;
    public Dictionary<Guid, LotteryParticipantDto> Participants { get; set; }
}