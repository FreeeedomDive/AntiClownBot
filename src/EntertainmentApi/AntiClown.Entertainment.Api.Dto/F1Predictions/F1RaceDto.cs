namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1RaceDto
{
    public Guid Id { get; set; }
    public int Season { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpened { get; set; }
    public bool IsSprint { get; set; }
    public F1RacePredictionConditionsDto Conditions { get; set; }
    public List<F1PredictionDto> Predictions { get; set; }
    public F1PredictionRaceResultDto Result { get; set; }

    public string FullName() => $"{Name}{(IsSprint ? " (спринт)" : string.Empty)}";
}