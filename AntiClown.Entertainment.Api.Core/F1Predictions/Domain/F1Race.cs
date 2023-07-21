namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class F1Race
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpened { get; set; }
    public List<F1Prediction> Predictions { get; set; }
    public F1PredictionRaceResult Result { get; set; }
}