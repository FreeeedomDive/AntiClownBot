using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class GiveLootBox : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;

        public GiveLootBox(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "lootbox";

        public string Execute(MessageCreateEventArgs e)
        {
            var args = e.Message.Content.Split();
            var give = args[1].Equals("add");
            var user = ulong.Parse(args[2]);
            var result = give ? ItemsApi.AddLootBox(user) : ItemsApi.RemoveLootBox(user);
            return "ok";
        }
    }
}