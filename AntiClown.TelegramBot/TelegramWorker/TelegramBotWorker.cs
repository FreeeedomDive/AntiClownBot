using AntiClown.Api.Client;
using AntiClown.Core.Dto.Exceptions;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Tokens;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelemetryApp.Api.Client.Log;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.TelegramBot.TelegramWorker;

public class TelegramBotWorker : ITelegramBotWorker
{
    public TelegramBotWorker(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownDataApiClient antiClownDataApiClient,
        ITelegramBotClient telegramBotClient,
        ILoggerClient loggerClient
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.telegramBotClient = telegramBotClient;
        this.loggerClient = loggerClient;
    }

    public async Task StartAsync()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
        };

        telegramBotClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions
        );

        await Task.Delay(-1);
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        await loggerClient.ErrorAsync(exception, "Telegram polling error");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: not null } message)
        {
            return;
        }

        var messageText = message.Text ?? "";
        if (messageText.StartsWith("/start"))
        {
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Чтобы привязать телеграм-аккаунт к дискорд-аккаунту, выполни команду web на сервере и введи UserId и Token сюда",
                cancellationToken: cancellationToken
            );
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "UserId:",
                cancellationToken: cancellationToken
            );
            return;
        }

        // пользователь вводит UserId
        if (!telegramToApiUserIds.TryGetValue(message.Chat.Id, out var apiUserId))
        {
            var isGuid = Guid.TryParse(message.Text, out var userId);
            if (!isGuid)
            {
                await telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "UserId должен быть гуидом",
                    cancellationToken: cancellationToken
                );
                return;
            }

            try
            {
                await antiClownApiClient.Users.ReadAsync(userId);
                telegramToApiUserIds[message.Chat.Id] = userId;
                await telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Token:",
                    cancellationToken: cancellationToken
                );
            }
            catch (EntityNotFoundException)
            {
                await telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Такого пользователя не существует",
                    cancellationToken: cancellationToken
                );
                return;
            }

            return;
        }
        
        // пользователь вводит токен
        try
        {
            await antiClownDataApiClient.Tokens.ValidateAsync(
                apiUserId, new TokenDto
                {
                    Token = messageText,
                }
            );
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Телеграм-аккаунт успешно привязан",
                cancellationToken: cancellationToken
            );
        }
        catch (UnauthorizedException)
        {
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный токен",
                cancellationToken: cancellationToken
            );
        }
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ITelegramBotClient telegramBotClient;
    private readonly ILoggerClient loggerClient;
    private readonly Dictionary<long, Guid> telegramToApiUserIds = new();
}