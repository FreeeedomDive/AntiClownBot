using System.Collections.Concurrent;
using System.Text;
using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.DiscordBot.Client;
using AntiClown.DiscordBot.Dto.Members;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Parties;
using Telegram.Bot;

namespace AntiClown.Telegram.Bot.Interactivity.Parties;

public class PartiesService : IPartiesService
{
    public PartiesService(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IAntiClownDiscordBotClient antiClownDiscordBotClient,
        ITelegramBotClient telegramBotClient
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.antiClownDiscordBotClient = antiClownDiscordBotClient;
        this.telegramBotClient = telegramBotClient;
    }

    public async Task CreateOrUpdateMessageAsync(Guid partyId)
    {
        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        var users = party.Participants.Any()
            ? await Task.WhenAll(party.Participants.Select(x => antiClownApiClient.Users.ReadAsync(x))) /* TODO: add api method for bulk read */
            : Array.Empty<UserDto>();
        var discordMembers = party.Participants.Any()
            ? await antiClownDiscordBotClient.DiscordMembers.GetDiscordMembersAsync(party.Participants.ToArray())
            : Array.Empty<DiscordMemberDto>();
        var messageText = CreateMessageText(party, discordMembers);

        var telegramBoundUsers = users.Where(x => x.TelegramId is not null).Select(x => x.TelegramId!.Value).ToArray();
        await Task.WhenAll(telegramBoundUsers.Select(x => SendMessageAsync(partyId, x, messageText)));
    }

    private static string CreateMessageText(PartyDto party, DiscordMemberDto[] discordMembers)
    {
        var discordMemberByUserId = discordMembers.ToDictionary(x => x.UserId);
        return new StringBuilder()
               .AppendLine($"Сбор пати {party.Name} {party.Description}")
               .AppendLine($"{party.Participants.Count} / {party.MaxPlayers}")
               .AppendLine(
                   party.Participants.Any()
                       ? string.Join(
                           "\n", party
                                 .Participants
                                 .Select((x, i) => $"{i + 1}. {(GetMemberName(x, discordMemberByUserId))}")
                       )
                       : "Пока никто не записался..."
               )
               .AppendLine()
               .AppendLine($"ID: {party.Id}")
               .AppendLine($"Создатель: {GetMemberName(party.CreatorId, discordMemberByUserId)}")
               .AppendLine(party.IsOpened ? string.Empty : "СБОР ЗАКРЫТ")
               .ToString();
    }

    private static string GetMemberName(Guid userId, Dictionary<Guid, DiscordMemberDto> discordMemberByUserId)
    {
        return discordMemberByUserId.TryGetValue(userId, out var member) ? member.ServerName ?? member.UserName ?? "" : "(чел, которого нет на сервере)";
    }

    private async Task SendMessageAsync(Guid partyId, long chatId, string messageText)
    {
        var messageKey = $"Party_{partyId}_{chatId}";
        if (partyIdToMessageId.TryGetValue(messageKey, out var messageId))
        {
            await telegramBotClient.EditMessageTextAsync(chatId, messageId, messageText);
            return;
        }

        var message = await telegramBotClient.SendTextMessageAsync(chatId, messageText);
        partyIdToMessageId.TryAdd(messageKey, message.MessageId);
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownDiscordBotClient antiClownDiscordBotClient;
    private readonly ITelegramBotClient telegramBotClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ConcurrentDictionary<string, int> partyIdToMessageId = new();
}