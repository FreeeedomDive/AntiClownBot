using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.DiscordBot.Extensions;

public static class F1SafetyCarsPredictionDtoExtensions
{
    public static string ToNumberedString(this F1SafetyCarsPredictionDto f1SafetyCarsPredictionDto)
    {
        return f1SafetyCarsPredictionDto switch
        {
            F1SafetyCarsPredictionDto.Zero => "0",
            F1SafetyCarsPredictionDto.One => "1",
            F1SafetyCarsPredictionDto.Two => "2",
            F1SafetyCarsPredictionDto.ThreePlus => "3+",
            _ => throw new ArgumentOutOfRangeException(nameof(f1SafetyCarsPredictionDto), f1SafetyCarsPredictionDto, null),
        };
    }
}