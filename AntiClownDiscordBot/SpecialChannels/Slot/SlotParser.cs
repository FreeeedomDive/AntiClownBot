using AntiClownBot.Models.SlotMachine;
using AntiClownBot.SpecialChannels.Slot.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.SpecialChannels.Slot
{
    public class SlotParser : SpecialChannelParser
    {
        public SlotParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            Commands = new List<ICommand>
            {
                new SlotPlay(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        public override async void Parse(MessageCreateEventArgs e)
        {
            if (!Commands.TryGetValue(e.Message.Content.Split(' ').First(), out var command))
            {
                return;
            }
            var message = command.Execute(e);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
        public override string Help(MessageCreateEventArgs e)
        {
            return "play [ставка]\n" + SlotMachine.Description();
        }
    }
}
