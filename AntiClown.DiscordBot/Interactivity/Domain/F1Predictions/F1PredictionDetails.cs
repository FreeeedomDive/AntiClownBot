using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.DiscordBot.Interactivity.Domain.F1Predictions;

public class F1PredictionDetails
{
    public Guid RaceId { get; set; }
    public List<F1DriverDto> Classification { get; set; }
    public F1DriverDto? FirstDnf { get; set; }
}