using System.Threading.Tasks;
using AntiClownBot.Models.Race;

namespace AntiClownBot.Events.RaceEvent
{
    public class RaceEvent: BaseEvent
    {

        public override async void ExecuteAsync()
        {
            if (Config.CurrentRace != null) return;

            var joinableMessage = await TellBackStory();
            await joinableMessage.CreateReactionAsync(Utility.Emoji(":monkaSTEER:"));

            Config.CurrentRace = new RaceModel
            {
                JoinableMessageId = joinableMessage.Id
            };
            await Task.Delay(10 * 60 * 1000);
            Config.CurrentRace.StartRace();
        }

        protected override string BackStory() => "@here Начинаем гоночку!!!" +
                                                 "\nСоревнуйтесь друг с другом и, главное, со мной, ведь тот, кто сможет обойти меня (и попасть в топ-10), получит социальный рейтинг" +
                                                 "\nРаспределение рейтинга - 250, 180, 150, 120, 100, 80, 60, 40, 20, 10" +
                                                 "\nДля получения рейтинга обязательно нужно быть впереди меня" +
                                                 $"\nЖми {Utility.Emoji(":monkaSTEER:")}, чтобы участвовать." +
                                                 "\nСтарт через 10 минут\n";
    }
}