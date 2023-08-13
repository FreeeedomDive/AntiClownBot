namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionDto
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public F1DriverDto TenthPlacePickedDriver { get; set; }
    public F1DriverDto FirstDnfPickedDriver { get; set; }
}