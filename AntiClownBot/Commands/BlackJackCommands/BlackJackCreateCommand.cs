using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackCreateCommand : BaseCommand
    {
        public BlackJackCreateCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if(Config.CurrentBlackJack != null)
            {
                await e.Message.RespondAsync("BlackJack already started");
                return;
            }
            Config.CurrentBlackJack = new Models.BlackJack.BlackJack();
            await e.Message.RespondAsync("BlackJack started, join!");
            Config.Save();
        }

        public override string Help()
        {
            return "Creating new BlackJack \"table\" which u can join";
        }
    }
}