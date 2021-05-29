using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class ConnectToChannel : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public ConnectToChannel(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "connect";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var args = string.Join(" ", e.Message.Content.Split(' ').Skip(1));
            if (!ulong.TryParse(args, out ulong id))
            {
                return "Что ты тут высрал?";
            }

            if (!DiscordClient.Guilds[Constants.GuildId].Channels.ContainsKey(id))
            {
                return "Такого канала не существует";
            }

            var channel = DiscordClient.Guilds[Constants.GuildId].Channels[id];
            if (channel.Type != ChannelType.Voice)
            {
                return "Это не голосовой канал";
            }

            if (!Voice.TryConnect(channel, out var connection))
            {
                return "Бот уже есть в другом канале";
            }

            return "Зашёл по кайфу";
        }
    }
}
