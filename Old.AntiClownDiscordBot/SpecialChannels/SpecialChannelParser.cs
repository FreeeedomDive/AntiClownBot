using AntiClownBot.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;

namespace AntiClownBot.SpecialChannels
{
    public abstract class SpecialChannelParser
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        protected Dictionary<string, ICommand> Commands;

        protected SpecialChannelParser(DiscordClient client, Configuration configuration)
        {
            DiscordClient = client;
            Config = configuration;
        }
        public abstract void Parse(MessageCreateEventArgs e);
        public abstract string Help(MessageCreateEventArgs e);
    }
}
