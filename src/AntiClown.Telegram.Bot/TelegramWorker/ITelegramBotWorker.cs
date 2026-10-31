namespace AntiClown.TelegramBot.TelegramWorker;

public interface ITelegramBotWorker
{
    Task StartAsync(CancellationToken cancellationToken);
}