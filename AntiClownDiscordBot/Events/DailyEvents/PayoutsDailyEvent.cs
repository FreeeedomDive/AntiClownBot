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

            var allUsers = UsersApi.GetAllUsers();

            UsersApi.BulkChangeUserBalance(allUsers.Users, 150, "Ежедневные выплаты");
            UsersApi.DailyReset();
        }

        protected override string BackStory()
        {
            return "Ежедневные выплаты!!! Сброс цены реролла в магазине и бесплатных распознавателей!!!";
        }
    }
}
