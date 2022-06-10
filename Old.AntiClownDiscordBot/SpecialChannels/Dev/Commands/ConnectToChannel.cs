using System.Linq;
using System.Threading;
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

        public string Execute(MessageCreateEventArgs e)
        {
            var args = string.Join(" ", e.Message.Content.Split(' ').Skip(1));
            if (!ulong.TryParse(args, out var id))
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

            new Thread(() =>
            {
                if (Voice.TryConnect(channel, out var connection)) 
                    return;
                if (connection.TargetChannel.Id == channel.Id) 
                    return;
                connection.Disconnect();
                Voice.TryConnect(channel, out connection);
            })
            {
                IsBackground = true
            }.Start();
            return "Зашёл по кайфу";
        }
    }
}