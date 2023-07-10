namespace AntiClown.Entertainment.Api.Core.Common;

/// <summary>
///     TODO: Впоследствии вынести бы в отдельный управляемый сервис
/// </summary>
public static class Constants
{
    /// <summary>
    ///     Время ожидания перед стартом эвента угадайки числа - 10 минут
    /// </summary>
    public const int GuessNumberEventWaitingTimeInMilliseconds = 10 * 60 * 1000;

    /// <summary>
    ///     Время ожидания перед стартом лотереи - 10 минут
    /// </summary>
    public const int LotteryEventWaitingTimeInMilliseconds = 10 * 60 * 1000;

    /// <summary>
    ///     Минимальное количество передаваемых скам-койнов в эвенте перекачки
    /// </summary>
    public const int TransfusionMinimumExchange = 50;

    /// <summary>
    ///     Максимальное количество передаваемых скам-койнов в эвенте перекачки
    /// </summary>
    public const int TransfusionMaximumExchange = 200;
}