using AntiClownDiscordBotVersion2.Models.GuessNumber;

namespace AntiClownDiscordBotVersion2.EventServices;

public interface IGuessNumberService
{
    void CreateGuessNumberGame(ulong messageId);
    GuessNumberGame? CurrentGame { get; set; }
}