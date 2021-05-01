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
    class F1CommandParser : BaseCommand
    {
        public F1CommandParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            switch(messageArgs.Last())
            {
                default:
                    await e.Message.RespondAsync("https://docs.google.com/spreadsheets/d/1-_FdarbFwNoQ0DGii1K5Sl45mEprO-Z5PD2p_mStje4/edit?usp=sharing");
                    break;
            }
        }

        public override string Help()
        {
            return "Может принимать в себя параметры(нет)\n" +
                "без параметров : возвращает ссылку на УДОБНЫЙ гугл док с результатами f1(автор: Лучший снайпер данного дискорда)\n";

        }
    }
}
