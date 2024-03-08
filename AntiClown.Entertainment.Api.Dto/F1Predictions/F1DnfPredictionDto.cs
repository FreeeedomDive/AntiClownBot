namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1DnfPredictionDto
{
    public F1DriverDto[]? DnfPickedDrivers { get; set; }
    public bool NoDnfPredicted { get; set; }
}