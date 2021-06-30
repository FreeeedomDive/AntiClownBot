using System;
using System.Threading.Tasks;
using AntiClownBot.Models.GuessNumber;

namespace AntiClownBot.Events.GuessNumberEvent
{
    public class GuessNumberEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
            if (Config.CurrentGuessNumberGame != null)
                return;
            var text = BackStory();
            var message = await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);
            await message.CreateReactionAsync(Utility.Emoji(":one:"));
            await message.CreateReactionAsync(Utility.Emoji(":two:"));
            await message.CreateReactionAsync(Utility.Emoji(":three:"));
            await message.CreateReactionAsync(Utility.Emoji(":four:"));
            Config.CurrentGuessNumberGame = new GuessNumberGame(message.Id);
            Config.Save();
            await Task.Delay(10 * 60 * 1000);
            Config.CurrentGuessNumberGame.MakeResult();
            Config.CurrentGuessNumberGame = null;
        }

        protected override string BackStory()
        {
            return "@everyone Я загадал число, угадайте его!!\n" +
                   "У вас 10 минут";
        }
    }
}
