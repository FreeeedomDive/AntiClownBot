using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Core.Dto.Exceptions;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Data.Api.Dto.Tokens;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Parties;
using AntiClown.Telegram.Bot.Interactivity.Parties;
using AntiClown.TelegramBot.TelegramWorker;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Telegram.Bot.TelegramWorker;

public class TelegramBotWorker(
    IAntiClownApiClient antiClownApiClient,
    IAntiClownDataApiClient antiClownDataApiClient,
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IPartiesService partiesService,
    ITelegramBotClient telegramBotClient,
    ILogger<TelegramBotWorker> logger
)
    : ITelegramBotWorker
{
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

    private Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Telegram polling error");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        /* TODO: divide to different handlers */

        if (update.CallbackQuery is not null)
        {
            await HandleCallbackButton(update);
            return;
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
            // TODO: втащить красивый обработчик команд
            if (await CreateParty(messageText, userWithCurrentTelegramId))
            {
                return;
            }

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
            await antiClownApiClient.Users.BindTelegramAsync(apiUserId, message.Chat.Id);
            await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Телеграм-аккаунт успешно привязан",
                cancellationToken: cancellationToken
            );
            telegramToApiUserIds.Remove(message.Chat.Id);
            logger.LogInformation("User {userId} has bound telegram account {telegramUserId}", apiUserId, message.Chat.Id);
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

    private async Task<bool> CreateParty(string messageText, UserDto userId)
    {
        var allowedGames = PartyGameOptions.AllowedParties.ToDictionary(x => x.CommandPrefix);

        var args = messageText.Split(' ');
        var commandName = args[0];

        if (!allowedGames.TryGetValue(commandName, out var partyGameOptions))
        {
            return false;
        }

        var description = args.Length > 1 ? string.Join(' ', args.Skip(1)) : string.Empty;

        await antiClownEntertainmentApiClient.Parties.CreateAsync(
            new CreatePartyDto
            {
                CreatorId = userId.Id,
                MaxPlayers = partyGameOptions.MaxPlayers,
                Name = partyGameOptions.PartyName,
                RoleId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, partyGameOptions.RoleIdKey),
                Description = description,
                AuthorAutoJoin = true,
            }
        );
        return true;
    }

    private async Task HandleCallbackButton(Update update)
    {
        var userId = update.CallbackQuery!.From.Id;
        if (update.CallbackQuery.Data?.StartsWith("JoinParty") ?? false)
        {
            var partyId = Guid.Parse(update.CallbackQuery.Data.Split("_")[1]);
            await partiesService.JoinPartyAsync(partyId, userId);
            return;
        }

        if (update.CallbackQuery.Data?.StartsWith("LeaveParty") ?? false)
        {
            var partyId = Guid.Parse(update.CallbackQuery.Data.Split("_")[1]);
            await partiesService.LeavePartyAsync(partyId, userId);
        }
    }

    private readonly Dictionary<long, Guid> telegramToApiUserIds = new();
}