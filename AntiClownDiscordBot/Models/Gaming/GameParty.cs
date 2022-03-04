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
        private bool _isOpened = true;
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
                Content = MessageContent()
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

        private async Task<DiscordMessage> GetPartyMessageAsync() => await Utility.Client
            .Guilds[Constants.GuildId]
            .Channels[Constants.PartyChannelId]
            .GetMessageAsync(MessageId);

        private DiscordRole GetGameRoleById() => AttachedRoleId == null
            ? null
            : Utility.Client
                .Guilds[Constants.GuildId]
                .GetRole(AttachedRoleId.Value);

        private string MessageContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{RoleMention()} СБОР ПАТИ {Description}").Append('\n');
            for (var i = 0; i < Math.Max(MaxPlayersCount, Players.Count); i++)
            {
                var position = i < MaxPlayersCount ? $"{i + 1}. " : "В ОЧЕРЕДИ - ";
                stringBuilder.Append(position);
                var player = i < Players.Count
                    ? Configuration.GetServerMember(Players[i]).ServerOrUserName()
                    : "";
                stringBuilder.Append(player).Append('\n');
            }

            if (!_isOpened) stringBuilder.Append("СБОР ЗАКРЫТ");

            return stringBuilder.ToString();
        }

        private async void CreatePingIfFullParty()
        {
            if (Players.Count != MaxPlayersCount) return;
            var readyPlayersMentions = Players.Take(MaxPlayersCount)
                .Select(playerId => Configuration.GetServerMember(playerId).Mention);
            var messageBuilder = new DiscordMessageBuilder
            {
                Content = $"НАБРАНО ПОЛНОЕ ПАТИ\n{string.Join("\n", readyPlayersMentions)}"
            };
            messageBuilder.WithAllowedMention(UserMention.All);

            await Message.RespondAsync(messageBuilder);
        }

        public void Destroy(ulong userId)
        {
            if (userId != CreatorId) return;

            var config = Configuration.GetConfiguration();
            config.OpenParties.Remove(MessageId);
            _isOpened = false;
            UpdateMessage();
            Configuration.GetConfiguration().UpdatePartyObservers();
        }
    }
}