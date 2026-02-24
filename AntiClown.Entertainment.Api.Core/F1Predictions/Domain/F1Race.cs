using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class F1Race
{
    public Guid Id { get; set; }
    public int Season { get; set; }
    public string Name { get; set; }
    public bool IsSprint { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpened { get; set; }
    public List<F1Prediction> Predictions { get; set; }
    public PredictionConditions Conditions { get; set; }
    public F1PredictionRaceResult Result { get; set; }
}