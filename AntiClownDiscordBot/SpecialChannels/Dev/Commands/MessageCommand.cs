using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class MessageCommand : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public MessageCommand(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "send";

        public string Execute(MessageCreateEventArgs e)
        {
            var args = e.Message.Content.Split(' ').Skip(1).ToList();
            if (args.Count < 2)
                return "Так дела не делаются";
            if (!ulong.TryParse(args.First(), out var channelId))
            {
                return "Найс канал, чел";
            }

            if (!DiscordClient.Guilds[Constants.GuildId].Channels.ContainsKey(channelId))
            {
                return "Нет такого канала";
            }
            var message = string.Join(" ", args.Skip(1));
            var channel = DiscordClient.Guilds[Constants.GuildId].Channels[channelId];
            if (channel.Type != ChannelType.Text)
            {
                return "Это не текстовый чат";
            }

            channel.SendMessageAsync(message);
            return "Отправил";
        }
    }
}
