using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.Helpers;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace AntiClownBot.Models.Gaming
{
    public class GameParty
    {
        private bool isOpened = true;
        public string Description;
        public ulong CreatorId { get; init; }
        public int MaxPlayersCount { get; set; }
        public List<ulong> Players { get; }

        public ulong? AttachedRoleId { get; set; }

        public ulong MessageId { get; set; }

        public DateTime CreationDate { get; init; }

        private DiscordMessage message;
        [JsonIgnore] public DiscordMessage Message => message ??= GetPartyMessageAsync().GetAwaiter().GetResult();

        private DiscordRole attachedRole;
        [JsonIgnore] public DiscordRole AttachedRole => attachedRole ??= GetGameRoleById();

        public GameParty()
        {
            Players = new List<ulong>();
        }

        public void JoinParty(ulong discordId)
        {
            if (Players.Contains(discordId)) return;
            Players.Add(discordId);
            UpdateMessage();
            CreatePingIfFullParty();
            Configuration.GetConfiguration().UpdatePartyObservers();
        }

        public void LeaveParty(ulong discordId)
        {
            if (!Players.Contains(discordId)) return;
            Players.Remove(discordId);
            UpdateMessage();
            Configuration.GetConfiguration().UpdatePartyObservers();
        }

        public async Task CreateMessage(MessageCreateEventArgs e)
        {
            var messageBuilder = new DiscordMessageBuilder
            {
                Content = RoleMention(),
                Embed = MessageContent()
            };
            messageBuilder.WithAllowedMention(DSharpPlus.Entities.RoleMention.All);
            var createdMessage = await e.Message.RespondAsync(messageBuilder);
            MessageId = createdMessage.Id;
        }

        private string RoleMention() => AttachedRoleId == null
            ? "@here"
            : AttachedRole.Mention;

        private async void UpdateMessage()
        {
            await Message.ModifyAsync(MessageContent());
        }

        private async Task<DiscordMessage> GetPartyMessageAsync()
        {
            DiscordMessage partyMessage;
            try
            {
                partyMessage = await Utility.Client
                    .Guilds[Constants.GuildId]
                    .Channels[Constants.PartyChannelId]
                    .GetMessageAsync(MessageId);
            }
            catch
            {
                partyMessage = await Utility.Client
                    .Guilds[Constants.GuildId]
                    .Channels[879784704696549498]
                    .GetMessageAsync(MessageId);
            }

            return partyMessage;
        }

        private DiscordRole GetGameRoleById() => AttachedRoleId == null
            ? null
            : Utility.Client
                .Guilds[Constants.GuildId]
                .GetRole(AttachedRoleId.Value);

        private DiscordEmbed MessageContent()
        {
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"СБОР ПАТИ {Description}")
                .WithColor(GetGameRoleById()?.Color ?? DiscordColor.White);

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < MaxPlayersCount; i++)
            {
                stringBuilder.Append($"{i + 1}. ");
                var player = i < Players.Count
                    ? Configuration.GetServerMember(Players[i]).ServerOrUserName()
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
                    var player = Configuration.GetServerMember(Players[i]).ServerOrUserName();
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

        private async void CreatePingIfFullParty()
        {
            if (Players.Count != MaxPlayersCount) return;
            var readyPlayersMentions = Players.Take(MaxPlayersCount)
                .Select(playerId => Configuration.GetServerMember(playerId).Mention);
            var messageBuilder = new DiscordMessageBuilder
            {
                Content =
                    $"НАБРАНО ПОЛНОЕ ПАТИ (за {Utility.GetTimeDiff(CreationDate)})\n{string.Join("\n", readyPlayersMentions)}"
            };
            messageBuilder.WithAllowedMention(UserMention.All);

            await Message.RespondAsync(messageBuilder);
            if (MaxPlayersCount >= 3)
            {
                SaveStats();
            }
        }

        private void SaveStats()
        {
            var config = Configuration.GetConfiguration();
            var seconds = Utility.GetTimeSpan(CreationDate).TotalSeconds;
            config.PartyStats.FastestPartyInSeconds = Math.Min(config.PartyStats.FastestPartyInSeconds, seconds);
            config.PartyStats.TotalFullParties++;
            config.PartyStats.TotalSeconds += seconds;
            config.Save();
        }

        public void Destroy(ulong userId)
        {
            if (userId != CreatorId) return;

            var config = Configuration.GetConfiguration();
            config.OpenParties.Remove(MessageId);
            isOpened = false;
            UpdateMessage();
            config.UpdatePartyObservers();
        }
    }
}