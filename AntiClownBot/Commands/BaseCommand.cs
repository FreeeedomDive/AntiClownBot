using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands
{
    public abstract class BaseCommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;

        protected BaseCommand(DiscordClient client, Configuration configuration)
        {
            DiscordClient = client;
            Config = configuration;
        }

        public abstract void Execute(MessageCreateEventArgs e, SocialRatingUser user);
        public abstract string Help();
    }
}