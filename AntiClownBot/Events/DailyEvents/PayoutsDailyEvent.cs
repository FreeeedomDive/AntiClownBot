
namespace AntiClownBot.Events.DailyEvents
{
    public class PayoutsDailyEvent : BaseEvent
    {
        public override int EventCooldown => 10 * 1000;
        public override async void ExecuteAsync()
        {
            var text = BackStory();
            await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);
            foreach (var user in Config.Users)
            {
                user.Value.ChangeRating(150);
            }
        }

        protected override string BackStory()
        {
            return "Всем выплаты!!!";
        }
    }
}
