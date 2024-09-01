using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Core.Dto.Exceptions;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Tokens;
using AntiClown.Telegram.Bot.Caches.Users;
using AntiClown.Telegram.Bot.Interactivity.Parties;
using AntiClown.TelegramBot.TelegramWorker;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelemetryApp.Api.Client.Log;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Telegram.Bot.TelegramWorker;

public class TelegramBotWorker : ITelegramBotWorker
{
    public TelegramBotWorker(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownDataApiClient antiClownDataApiClient,
        IPartiesService partiesService,
        IUsersCache usersCache,
        ITelegramBotClient telegramBotClient,
        ILoggerClient loggerClient
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.partiesService = partiesService;
        this.usersCache = usersCache;
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
        /* TODO: divide to different handlers */

        if (update.CallbackQuery is not null)
        {
            var userId = update.CallbackQuery.From.Id;
            if (update.CallbackQuery.Data?.StartsWith("JoinParty") ?? false)
            {
                var partyId = Guid.Parse(update.CallbackQuery.Data.Split("_")[1]);
                await partiesService.JoinPartyAsync(partyId, userId);
            }

            if (update.CallbackQuery.Data?.StartsWith("LeaveParty") ?? false)
            {
                var partyId = Guid.Parse(update.CallbackQuery.Data.Split("_")[1]);
                await partiesService.LeavePartyAsync(partyId, userId);
            }
        }

        if (update.Message is not { Text: not null } message)
        {
            return;
        }

        var messageText = message.Text ?? "";

        var userWithCurrentTelegramId = (await antiClownApiClient.Users.FindAsync(
            new UserFilterDto
            {
                TelegramId = message.Chat.Id,
            }
        )).FirstOrDefault();
        if (userWithCurrentTelegramId is not null)
        {
            // TODO: пользователь уже привязал телеграм, здесь будет обработчик будущих команд
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                $"Привязанный UserId: {userWithCurrentTelegramId.Id}",
                cancellationToken: cancellationToken
            );
            return;
        }

        if (messageText.StartsWith("/start"))
        {
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Чтобы привязать телеграм-аккаунт к дискорд-аккаунту, выполни команду web на сервере и введи UserId и Token",
                cancellationToken: cancellationToken
            );
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "UserId:",
                cancellationToken: cancellationToken
            );
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
            await antiClownApiClient.Users.BindTelegramAsync(apiUserId, message.Chat.Id);
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Телеграм-аккаунт успешно привязан",
                cancellationToken: cancellationToken
            );
            usersCache.BindTelegram(message.Chat.Id, apiUserId);
            telegramToApiUserIds.Remove(message.Chat.Id);
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
    private readonly ILoggerClient loggerClient;
    private readonly IPartiesService partiesService;
    private readonly IUsersCache usersCache;
    private readonly ITelegramBotClient telegramBotClient;
    private readonly Dictionary<long, Guid> telegramToApiUserIds = new();
}