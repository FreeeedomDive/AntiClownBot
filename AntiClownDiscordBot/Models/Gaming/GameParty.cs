using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Models.Gaming
{
    public class GameParty
    {
        private bool _isOpened = true;
        public string Description;
        public ulong CreatorId { get; init; }
        public int MaxPlayersCount { get; set; }
        public List<ulong> Players { get; }

        public DiscordRole AttachedRole { get; set; }

        public DiscordMessage Message;

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
            Message = await e.Message.RespondAsync(messageBuilder);
        }

        private string RoleMention() => AttachedRole != null ? AttachedRole.Mention : "@here";

        private async void UpdateMessage() => await Message.ModifyAsync(MessageContent());

        private string MessageContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{RoleMention()} СБОР ПАТИ {Description}").Append('\n');
            for (var i = 0; i < Math.Max(MaxPlayersCount, Players.Count); i++)
            {
                var position = i < MaxPlayersCount ? $"{i + 1}. " : "В ОЧЕРЕДИ - ";
                stringBuilder.Append(position);
                var player = i < Players.Count
                    ? Utility.Client.Guilds[Constants.GuildId].GetMemberAsync(Players[i]).Result.Nickname
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
                .Select(playerId => Utility.Client.Guilds[Constants.GuildId].GetMemberAsync(playerId).Result.Mention);
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
            config.OpenParties.Remove(Message.Id);
            _isOpened = false;
            UpdateMessage();
            Configuration.GetConfiguration().UpdatePartyObservers();
        }
    }
}