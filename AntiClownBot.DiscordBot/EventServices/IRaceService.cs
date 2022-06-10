using AntiClownDiscordBotVersion2.Models.Race;

namespace AntiClownDiscordBotVersion2.EventServices;

public interface IRaceService
{
    Task CreateRaceAsync(ulong messageId);
    RaceModel? Race { get; set; }
}