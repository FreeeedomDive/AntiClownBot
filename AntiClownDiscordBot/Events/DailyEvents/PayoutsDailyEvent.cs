using ApiWrapper.Wrappers;

namespace AntiClownBot.Events.DailyEvents
{
    public class PayoutsDailyEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
            var text = BackStory();
            await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);

            var allUsers = UsersWrapper.GetAllUsers();

            UsersWrapper.BulkChangeUserBalance(allUsers.Users, 150, "Ежедневные выплаты");
        }

        protected override string BackStory()
        {
            return "Всем выплаты!!!";
        }
    }
}
