namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Transfusion;

public class TransfusionEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.Transfusion;

    public Guid DonorUserId { get; set; }
    public Guid RecipientUserId { get; set; }
    public int Exchange { get; set; }

    public static TransfusionEvent Create()
    {
        return new TransfusionEvent
        {
            Id = Guid.NewGuid(),
            Finished = true,
            EventDateTime = DateTime.Now,
        };
    }
}