using System.Collections.Concurrent;
using System.Text;
using AntiClown.DiscordBot.Client;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Parties;
using AntiClown.Telegram.Bot.Caches.Users;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AntiClown.Telegram.Bot.Interactivity.Parties;

public class PartiesService : IPartiesService
{
    public PartiesService(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IAntiClownDiscordBotClient antiClownDiscordBotClient,
        ITelegramBotClient telegramBotClient,
        IUsersCache usersCache,
        ILogger<PartiesService> logger
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.antiClownDiscordBotClient = antiClownDiscordBotClient;
        this.telegramBotClient = telegramBotClient;
        this.usersCache = usersCache;
        this.logger = logger;
    }

    public async Task CreateOrUpdateMessageAsync(Guid partyId)
    {
        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        var messageText = CreateMessageText(party);

        var discordRoleMembersUsersIds = (await antiClownDiscordBotClient.DiscordMembers.FindByRoleIdAsync(party.RoleId))
                                         .Where(x => x is not null)
                                         .Select(x => x!.UserId)
                                         .ToArray();
        var usersToNotify = discordRoleMembersUsersIds.Select(x => usersCache.TryGetUser(x))
                                                      .Where(x => x?.TelegramId is not null)
                                                      .Select(x => x!.TelegramId!.Value)
                                                      .ToArray();
        await Task.WhenAll(usersToNotify.Select(x => SendMessageAsync(party, x, messageText)));
    }

    public async Task JoinPartyAsync(Guid partyId, long userId)
    {
        var user = usersCache.TryGetUser(userId);
        if (user is null)
        {
            return;
        }

        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        if (!party.IsOpened)
        {
            return;
        }

        await antiClownEntertainmentApiClient.Parties.JoinAsync(partyId, user.Id);
    }

    public async Task LeavePartyAsync(Guid partyId, long userId)
    {
        var user = usersCache.TryGetUser(userId);
        if (user is null)
        {
            return;
        }

        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        if (!party.IsOpened)
        {
            return;
        }

        await antiClownEntertainmentApiClient.Parties.LeaveAsync(partyId, user.Id);
    }

    private string CreateMessageText(PartyDto party)
    {
        return new StringBuilder()
               .AppendLine($"Сбор пати {party.Name} {party.Description}")
               .AppendLine($"{party.Participants.Count} / {party.MaxPlayers} игроков")
               .AppendLine(
                   party.Participants.Any()
                       ? string.Join(
                           "\n", party
                                 .Participants
                                 .Select((x, i) => $"{i + 1}. {GetMemberName(x)}")
                       )
                       : "Пока никто не записался..."
               )
               .AppendLine()
               .AppendLine($"ID: {party.Id}")
               .AppendLine($"Создатель: {GetMemberName(party.CreatorId)}")
               .AppendLine(party.IsOpened ? string.Empty : "СБОР ЗАКРЫТ")
               .ToString();
    }

    private string GetMemberName(Guid userId)
    {
        var member = usersCache.TryGetDiscordMember(userId);
        return member is not null ? member.ServerName ?? member.UserName ?? "" : "(чел, которого нет на сервере)";
    }

    private async Task SendMessageAsync(PartyDto party, long chatId, string messageText)
    {
        try
        {
            var partyId = party.Id;
            var messageKey = $"Party_{partyId}_{chatId}";
            var inlineButtons = new[]
            {
                new InlineKeyboardButton("Присоединиться")
                {
                    CallbackData = $"JoinParty_{partyId}",
                },
                new InlineKeyboardButton("Выйти")
                {
                    CallbackData = $"LeaveParty_{partyId}",
                },
            };
            var markup = party.IsOpened ? new InlineKeyboardMarkup(inlineButtons) : null;
            if (partyIdToMessageId.TryGetValue(messageKey, out var messageId))
            {
                await telegramBotClient.EditMessageTextAsync(chatId, messageId, messageText, replyMarkup: markup);
                return;
            }

            var message = await telegramBotClient.SendTextMessageAsync(chatId, messageText, replyMarkup: markup);
            partyIdToMessageId.TryAdd(messageKey, message.MessageId);
            logger.LogInformation("Successfully sent message {chatId} with partyId {partyId}", chatId, partyId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send party message to {userId}", chatId);
        }
    }

    private readonly IAntiClownDiscordBotClient antiClownDiscordBotClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<PartiesService> logger;
    private readonly ConcurrentDictionary<string, int> partyIdToMessageId = new();
    private readonly ITelegramBotClient telegramBotClient;
    private readonly IUsersCache usersCache;
}