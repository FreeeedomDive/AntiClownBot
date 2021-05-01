using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.F1
{
    public class F1CommandParser : BaseCommand
    {
        private readonly Dictionary<string,IF1Parser> _f1Parsers;
        public F1CommandParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            _f1Parsers = new List<IF1Parser>
            {
                new F1QualiParser(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var message = e.Message.Content;
            var messageArgs = message.Split(' ').Skip(1).ToList();
            if(!messageArgs.Any())
            {
                await e.Message.RespondAsync("https://docs.google.com/spreadsheets/d/1-_FdarbFwNoQ0DGii1K5Sl45mEprO-Z5PD2p_mStje4/edit?usp=sharing");
                return;
            }
            if(!_f1Parsers.TryGetValue(messageArgs.First(), out var parser))
            {
                await e.Message.RespondAsync("Чел, такой команды нет");
                return;
            }
            var result = parser.Execute(user, messageArgs.Skip(1));
            await e.Message.RespondAsync(result);
        }

        public override string Help()
        {
            return "Может принимать в себя параметры(нет)\n" +
                "без параметров : возвращает ссылку на УДОБНЫЙ гугл док с результатами f1(автор: Лучший снайпер данного дискорда)\n";

        }
    }
}
