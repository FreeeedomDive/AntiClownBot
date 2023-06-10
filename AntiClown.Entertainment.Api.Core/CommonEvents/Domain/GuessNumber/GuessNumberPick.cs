namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;

public enum GuessNumberPick
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    /// <summary>
    ///     Impossible to pick, but possible to get as correct answer
    /// </summary>
    Five = 5,
}