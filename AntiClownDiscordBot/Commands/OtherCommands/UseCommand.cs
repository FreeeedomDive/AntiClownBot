﻿using AntiClownBot.Helpers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands
{
    public class UseCommand : BaseCommand
    {
        public UseCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            await e.Message.RespondAsync($"{Utility.Emoji(":NOPERS:")} {Utility.Emoji(":NOPERS:")} {Utility.Emoji(":NOPERS:")}");
            
            //var args = e.Message.Content.Split(' ').ToList();
            //if (args.Count < 2)
            //{
            //    await e.Message.RespondAsync("Нормально команду пиши");
            //    return;
            //}
            // var itemToUse = args.Skip(1);
            // var item = AllItems.GetAllItems().FirstOrDefault(i => i.Name == string.Join(" ", itemToUse));
            // if (item == null)
            // {
            //     await e.Message.RespondAsync("Такого предмета не существует");
            //     return;
            // }
            // var msg = user.Use(item);
            // await e.Message.RespondAsync(msg);
        }

        public override string Help()
        {
            return "Использовать предмет из инвентаря\nИспользование:\n" +
                   "!use [item]";
        }
    }
}