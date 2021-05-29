using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.SpecialChannels.Dev.Commands;
using DSharpPlus;

namespace AntiClownBot.SpecialChannels.Dev
{
    public class DevParser : SpecialChannelParser
    {
        public DevParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            Commands = new List<ICommand>
            {
                new RunEvent(client, configuration),
                new PlayMusic(client, configuration),
                new ConnectToChannel(client, configuration),
                new Disconnect(client, configuration),
                new MessageCommand(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        public override string Help(MessageCreateEventArgs e)
        {
            return "Команды:\n" +
                   "runevent \n" +
                   "Параметры:\n" +
                   "closetributes/dailystats/guessnumber/lottery/malimali/opentributes/payouts/removecooldown/shop/transfusion\n" +
                   "play {filename}";
        }

        public override async void Parse(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!Commands.TryGetValue(e.Message.Content.Split(' ').First(), out var command))
            {
                return;
            }
            var message = command.Execute(e, user);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
    }
}
