namespace AntiClown.Messages.Dto.F1Predictions;

public class F1UserPredictionUpdatedMessageDto
{
    public Guid UserId { get; set; }
    public Guid RaceId { get; set; }
    public bool IsNew { get; set; }
}