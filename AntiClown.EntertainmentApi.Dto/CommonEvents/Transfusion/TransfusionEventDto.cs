namespace AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;

public class TransfusionEventDto : CommonEventBaseDto
{
    public Guid DonorUserId { get; set; }
    public Guid RecipientUserId { get; set; }
    public int Exchange { get; set; }
    public override CommonEventTypeDto Type => CommonEventTypeDto.Transfusion;
}