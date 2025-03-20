using System.Collections.Concurrent;
using System.Text;
using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.DiscordBot.Client;
using AntiClown.DiscordBot.Dto.Members;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Parties;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AntiClown.Telegram.Bot.Interactivity.Parties;

public class PartiesService(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IAntiClownDiscordBotClient antiClownDiscordBotClient,
    IAntiClownApiClient antiClownApiClient,
    ITelegramBotClient telegramBotClient,
    ILogger<PartiesService> logger
)
    : IPartiesService
{
    public async Task CreateOrUpdateMessageAsync(Guid partyId)
    {
        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        var partyUsersIds = party.Participants.Concat(new[] { party.CreatorId }).ToArray();
        var members = await antiClownDiscordBotClient.DiscordMembers.GetDiscordMembersAsync(partyUsersIds);
        var messageText = CreateMessageText(party, members);

        var users = (await antiClownApiClient.Users.ReadAllAsync()).ToDictionary(x => x.Id);
        var discordRoleMembersUsersIds = (await antiClownDiscordBotClient.DiscordMembers.FindByRoleIdAsync(party.RoleId))
                                         .Where(x => x is not null)
                                         .Select(x => x!.UserId)
                                         .ToArray();
        var usersToNotify = discordRoleMembersUsersIds.Select(x => users.GetValueOrDefault(x))
                                                      .Where(x => x?.TelegramId is not null)
                                                      .Select(x => x!.TelegramId!.Value)
                                                      .ToArray();
        await Task.WhenAll(usersToNotify.Select(x => SendMessageAsync(party, x, messageText)));
    }

    public async Task JoinPartyAsync(Guid partyId, long userId)
    {
        var users = await antiClownApiClient.Users.ReadAllAsync();
        var user = users.FirstOrDefault(x => x.TelegramId == userId);
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
        var users = await antiClownApiClient.Users.ReadAllAsync();
        var user = users.FirstOrDefault(x => x.TelegramId == userId);
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

    private static string CreateMessageText(PartyDto party, DiscordMemberDto?[] members)
    {
        return new StringBuilder()
               .AppendLine($"Сбор пати {party.Name} {party.Description}")
               .AppendLine($"{party.Participants.Count} / {party.MaxPlayers} игроков")
               .AppendLine(
                   party.Participants.Count > 0
                       ? string.Join(
                           "\n", party
                                 .Participants
                                 .Select((x, i) => $"{i + 1}. {GetMemberName(x, members)}")
                       )
                       : "Пока никто не записался..."
               )
               .AppendLine()
               .AppendLine($"ID: {party.Id}")
               .AppendLine($"Создатель: {GetMemberName(party.CreatorId, members)}")
               .AppendLine(party.IsOpened ? string.Empty : "СБОР ЗАКРЫТ")
               .ToString();
    }

    private static string GetMemberName(Guid userId, DiscordMemberDto?[] members)
    {
        var member = members.FirstOrDefault(x => x?.UserId == userId);
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

    private readonly IAntiClownApiClient antiClownApiClient = antiClownApiClient;
    private readonly ConcurrentDictionary<string, int> partyIdToMessageId = new();
}