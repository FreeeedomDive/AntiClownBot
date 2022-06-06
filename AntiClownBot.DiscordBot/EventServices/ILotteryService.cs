using AntiClownDiscordBotVersion2.Models.Lottery;

namespace AntiClownDiscordBotVersion2.EventServices;

public interface ILotteryService
{
    void CreateLottery(ulong messageId);
    Lottery? Lottery { get; set; }
}