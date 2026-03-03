namespace AntiClown.Messages.Dto.F1Predictions;

public class F1PredictionStartedMessageDto
{
    public Guid RaceId { get; set; }
    public string Name { get; set; }
}