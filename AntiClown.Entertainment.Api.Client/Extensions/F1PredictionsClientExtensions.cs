using AntiClown.Entertainment.Api.Client.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Client.Extensions;

public static class F1PredictionsClientExtensions
{
    public static Task CreateOrUpdateTeamAsync(this IF1PredictionsClient client, string teamName, string firstDriver, string secondDriver)
    {
        return client.CreateOrUpdateTeamAsync(
            new F1TeamDto
            {
                Name = teamName,
                FirstDriver = firstDriver,
                SecondDriver = secondDriver,
            }
        );
    }
}