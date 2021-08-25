using AntiClownBot.Models.Lohotron;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.Lohotron
{
    public class LohotronCommand : BaseCommand
    {
        public LohotronCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public override async void Execute(MessageCreateEventArgs e)
        {
            if (Config.DailyScamMachine.UsersId.Contains(e.Author.Id))
            {
                await e.Message.RespondAsync($"Чел, 2 раза нельзя! {Utility.Emoji(":peepoFinger:")}");
                return;
            }
            var prize = Config.DailyScamMachine.Play();
            Config.DailyScamMachine.UsersId.Add(e.Author.Id);
            switch (prize.Name)
            {
                case "Credits":
                    var count = ((CreditsLohotronPrize) prize).Count;
                    Config.ChangeBalance(e.Author.Id, count, "Лохотрон");
                    await e.Message.RespondAsync($"Ты получаешь {count} social credits");
                    return;
                case "Nothing":
                    await e.Message.RespondAsync($"Ты получаешь {Utility.Emoji(":peepoFinger:")}!");
                    return;
                default:
                    await e.Message.RespondAsync("Какой-то кал, всё сломалось");
                    return;
            }
        }

        public override string Help()
        {
            return "Ежедневно крутите лохотрон (1 раз)";
        }
    }
}
