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
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.DailyScamMachine.UsersId.Contains(user.DiscordId))
            {
                await e.Message.RespondAsync("Ты уже пользовался Лохотроном");
                return;
            }
            var prize = Config.DailyScamMachine.Play();
            Config.DailyScamMachine.UsersId.Add(user.DiscordId);
            switch (prize.Name)
            {
                case "Credits":
                    var count = ((CreditsLohotronPrize) prize).Count;
                    user.ChangeRating(count);
                    await e.Message.RespondAsync($"Ты получаешь {count} social credits");
                    return;
                case "Item":
                    var item = ((ItemLohotronPrize) prize).Item;
                    user.AddCustomItem(item);
                    await e.Message.RespondAsync($"Ты получаешь {Utility.ItemToString(item)}");
                    return;
                case "Nothing":
                    await e.Message.RespondAsync("Ты получаешь ничего!");
                    return;
                default:
                    await e.Message.RespondAsync("Какой-то кал, всё сломалось");
                    return;
            }
        }

        public override string Help()
        {
            return "Ежедневно крутите лохотрон(1 раз)";
        }
    }
}
