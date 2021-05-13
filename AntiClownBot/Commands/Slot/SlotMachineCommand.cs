using System.Linq;
using DSharpPlus;
using DSharpPlus.EventArgs;
using AntiClownBot.Models.SlotMachine;

namespace AntiClownBot.Commands.Slot
{
    public class SlotMachineCommand : BaseCommand
    {
        private SlotMachine slotMachine = new();
        
        public SlotMachineCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var splitMessage = e.Message.Content.Split(' ');
            if (splitMessage.Length != 2 
                || !int.TryParse(splitMessage[1], out var bet)
                || bet <= 0 
                || bet > user.SocialRating)
            {
                e.Message.RespondAsync("чел ты кого наебать вздумал");
                return;
            }

            var result = slotMachine.Play(bet);
            var textCells = string.Join(" ", result.Cells.Select(c => Utility.StringEmoji(c.Emoji)));

            var resultRatingChange = result.Win - bet;
            user.ChangeRating(resultRatingChange);
            
            e.Message.RespondAsync("Кручу верчу наебать хочу:\n" + textCells + "\nВыигрыш: " + result.Win);
        }

        public override string Help()
        {
            return SlotMachine.Description();
        }
    }
}