using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.FinanceCommands
{
    public class CoinsTransactionsCommand : BaseCommand
    {
        public CoinsTransactionsCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override void Execute(MessageCreateEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public override string Help()
        {
            throw new System.NotImplementedException();
        }
    }
}