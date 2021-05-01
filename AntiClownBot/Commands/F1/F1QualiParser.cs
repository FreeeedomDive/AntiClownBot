using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.F1
{
    public class F1QualiParser : IF1Parser
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public F1QualiParser(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "quali";
        public string Execute(SocialRatingUser user, IEnumerable<string> args)
        {
            return "Данная команда ещё в процессе разработки";
        }
    }
}
