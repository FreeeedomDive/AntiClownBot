using AntiClownBot.Models.User.Inventory;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.OtherCommands
{
    public class UseCommand : BaseCommand
    {
        public UseCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var args = e.Message.Content.Split(' ').ToList();
            if (args.Count != 2)
            {
                await e.Message.RespondAsync("Нормально команду пиши");
                return;
            }
            var item = AllItems.GetAllItems().Where(i => i.Name == args[1]).FirstOrDefault();
            if (item == null)
            {
                await e.Message.RespondAsync("Такого предмета не существует");
                return;
            }
            var msg = user.Use(item);
            await e.Message.RespondAsync(msg);
        }

        public override string Help()
        {
            return "Использовать предмет из инвентаря\nИспользование:\n" +
                   "!use [item]";
        }
    }
}
