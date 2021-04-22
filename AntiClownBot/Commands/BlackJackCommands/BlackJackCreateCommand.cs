using AntiClownBot.Models.BlackJack;
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
                await e.Message.RespondAsync("Стол уже создан");
                return;
            }
            Config.CurrentBlackJack = new BlackJack();
            await e.Message.RespondAsync("Стол для BlackJack создан");
            Config.Save();
        }

        public override string Help()
        {
            return "Создание нового \"стола\" для BlackJack";
        }
    }
}