namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Transfusion;

public class TransfusionEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.Transfusion;

    public Guid DonorUserId { get; set; }
    public Guid RecipientUserId { get; set; }
}