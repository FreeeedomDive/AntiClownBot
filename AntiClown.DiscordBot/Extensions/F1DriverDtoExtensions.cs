using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.DiscordBot.Extensions;

public static class F1DriverDtoExtensions
{
    public static string Trigram(this F1DriverDto f1Driver)
    {
        return f1Driver.ToString()[..3].ToUpper();
    }
}