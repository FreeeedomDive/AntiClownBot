namespace AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;

public class GuessNumberEventDto : CommonEventBaseDto
{
    public Dictionary<Guid, GuessNumberPickDto> Picks { get; set; } = new();
    public Dictionary<GuessNumberPickDto, List<Guid>> NumberToUsers { get; set; } = new();
    public GuessNumberPickDto Result { get; set; }
    public override CommonEventTypeDto Type => CommonEventTypeDto.GuessNumber;
}