using System.Text;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Models.Gaming
{
    public class GameParty
    {
        public GameParty(
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.guildSettingsService = guildSettingsService;
            Players = new List<ulong>();
        }

        public async Task JoinParty(ulong discordId)
        {
            if (Players.Contains(discordId)) return;
            Players.Add(discordId);
            await UpdateMessage();
            await CreatePingIfFullParty();
        }

        public async Task LeaveParty(ulong discordId)
        {
            if (!Players.Contains(discordId)) return;
            Players.Remove(discordId);
            await UpdateMessage();
        }

        public async Task<DiscordMessage> CreateMessage()
        {
            var role = await RoleMention();
            var embed = await CreateMessageEmbed();
            var messageBuilder = new DiscordMessageBuilder
            {
                Content = role,
                Embed = embed
            };
            messageBuilder.WithAllowedMention(DSharpPlus.Entities.RoleMention.All);
            var createdMessage =
                await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().PartyChannelId,
                    messageBuilder);
            MessageId = createdMessage.Id;

            return createdMessage;
        }

        private async Task<string> RoleMention() => AttachedRoleId == null
            ? "@here"
            : (await AttachedRole())!.Mention;

        private async Task UpdateMessage()
        {
            var content = await CreateMessageEmbed();
            var updatedMessage = await Message();
            await discordClientWrapper.Messages.ModifyAsync(updatedMessage, content);
        }

        private async Task<DiscordMessage> GetPartyMessageAsync()
        {
            var guild = guildSettingsService.GetGuildSettings();
            DiscordMessage partyMessage;
            try
            {
                partyMessage = await discordClientWrapper.Messages.FindMessageAsync(guild.PartyChannelId, MessageId);
            }
            catch
            {
                partyMessage =
                    await discordClientWrapper.Messages.FindMessageAsync(guild.HiddenTestChannelId, MessageId);
            }

            return partyMessage;
        }

        private async Task<DiscordRole?> GetGameRoleById()
        {
            if (AttachedRoleId == null)
            {
                return null;
            }

            return await discordClientWrapper.Roles.FindRoleAsync(AttachedRoleId.Value);
        }

        private async Task<DiscordEmbed> CreateMessageEmbed()
        {
            var role = await AttachedRole();
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"СБОР ПАТИ {Description}")
                .WithColor(role?.Color ?? DiscordColor.White);

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < MaxPlayersCount; i++)
            {
                stringBuilder.Append($"{i + 1}. ");
                var player = i < Players.Count
                    ? (await discordClientWrapper.Members.GetAsync(Players[i])).ServerOrUserName()
                    : "";
                stringBuilder.Append(player).Append('\n');
            }

            embedBuilder.AddField("Состав", stringBuilder.ToString());

            if (Players.Count > MaxPlayersCount)
            {
                stringBuilder.Clear();
                for (var i = MaxPlayersCount; i < Players.Count; i++)
                {
                    stringBuilder.Append($"{i + 1}. ");
                    var player = (await discordClientWrapper.Members.GetAsync(Players[i])).ServerOrUserName();
                    stringBuilder.Append(player).Append('\n');
                }

                embedBuilder.AddField("Очередь", stringBuilder.ToString());
            }

            if (!isOpened)
            {
                embedBuilder
                    .WithFooter("СБОР ЗАКРЫТ")
                    .WithColor(DiscordColor.Black);
            }

            return embedBuilder.Build();
        }

        private async Task CreatePingIfFullParty()
        {
            if (Players.Count != MaxPlayersCount) return;
            var readyPlayersMentionsTasks = Players
                .Take(MaxPlayersCount)
                .Select(playerId => discordClientWrapper.Members.GetAsync(playerId));
            var readyPlayers = await Task.WhenAll(readyPlayersMentionsTasks);
            var readyPlayersMentions = readyPlayers.Select(x => x.Mention);
            var messageBuilder = new DiscordMessageBuilder
            {
                Content =
                    $"НАБРАНО ПОЛНОЕ ПАТИ (за {Utility.GetTimeDiff(CreationDate)})\n{string.Join("\n", readyPlayersMentions.ToList())}"
            };
            messageBuilder.WithAllowedMention(UserMention.All);
            var alertMessage = await Message();
            await discordClientWrapper.Messages.RespondAsync(alertMessage, messageBuilder);
            if (MaxPlayersCount >= 3)
            {
                SaveStats();
            }
        }

        private void SaveStats()
        {
            var seconds = Utility.GetTimeSpan(CreationDate).TotalSeconds;
            OnStatsUpdate(seconds);
        }

        public async Task Destroy(ulong userId)
        {
            if (userId != CreatorId) return;

            OnPartyRemove(MessageId);
            isOpened = false;
            await UpdateMessage();
        }

        public static GameParty RestoreWithDependencies(
            GameParty deserialized,
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService,
            Action<ulong> onPartyRemove,
            Action<double> onStatsUpdate
        )
        {
            return new GameParty(discordClientWrapper, guildSettingsService)
            {
                Players = deserialized.Players,
                MaxPlayersCount = deserialized.MaxPlayersCount,
                CreatorId = deserialized.CreatorId,
                Description = deserialized.Description,
                AttachedRoleId = deserialized.AttachedRoleId,
                MessageId = deserialized.MessageId,
                CreationDate = deserialized.CreationDate,
                OnPartyRemove = onPartyRemove,
                OnStatsUpdate = onStatsUpdate
            };
        }

        public async Task<DiscordMessage> Message() => message ??= await GetPartyMessageAsync();
        public async Task<DiscordRole?> AttachedRole() => attachedRole ??= await GetGameRoleById();

        public string Description;
        public ulong CreatorId { get; init; }
        public int MaxPlayersCount { get; set; }
        public List<ulong> Players { get; private init; }

        public ulong? AttachedRoleId { get; set; }

        public ulong MessageId { get; set; }
        public DateTime CreationDate { get; init; }

        [JsonIgnore] public Func<Task> OnPartyObserverUpdate { get; init; }
        [JsonIgnore] public Action<ulong> OnPartyRemove { get; init; }
        [JsonIgnore] public Action<double> OnStatsUpdate { get; init; }

        [JsonIgnore] private bool isOpened = true;
        [JsonIgnore] private DiscordMessage? message;
        [JsonIgnore] private DiscordRole? attachedRole;

        [JsonIgnore] private readonly IDiscordClientWrapper discordClientWrapper;
        [JsonIgnore] private readonly IGuildSettingsService guildSettingsService;
    }
}