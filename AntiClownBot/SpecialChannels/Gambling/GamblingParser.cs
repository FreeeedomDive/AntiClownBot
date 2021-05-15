using AntiClownBot.SpecialChannels.Gambling.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.SpecialChannels.Gambling
{
    public class GamblingParser : SpecialChannelParser
    {
        public GamblingParser(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
            Commands = new List<ICommand>
            {
                new GambleCancel(client, configuration),
                new GambleClose(client, configuration),
                new GambleCurrent(client, configuration),
                new GambleCommand(client, configuration),
                new GambleResult(client, configuration),
                new GambleStartCustom(client, configuration),
                new GambleStart(client, configuration)
            }.ToDictionary(x => x.Name);
        }
        public override async void Parse(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!Commands.TryGetValue(e.Message.Content.Split(' ').First(), out var command))
            {
                await e.Message.RespondAsync("Чел, такой команды нет");
                return;
            }
            var message = command.Execute(e, user);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
        public override string Help(MessageCreateEventArgs e)
        {
            switch (e.Message.Content.Split(' ').Skip(1).First())
            {
                case "start":
                    return "Начало новой ставки\nМожно иметь только одну активную ставку\nИспользование:\n" +
                   "start [Название ставки]\n[Вариант 1]\n[Вариант 2]\n...\n[Вариант N]";
                case "startcustom":
                    return "Начало новой ставки с кастомными коэффициентами на каждый вариант\n" +
                   "Можно иметь только одну активную ставку\nИспользование:\n" +
                   "startcustom [Название ставки]\n[Вариант 1 [Коэффициент1]]\n[Вариант 2 [Коэффициент2]]\n..." +
                   "\n[Вариант N [КоэффициентN]]";
                case "result":
                    return "Закрытие текущей ставки с выбором победившего исхода событий\nИспользование:\nresult\n[Исход1]\n...\n[Исход N]\n" +
                   "Закрыть ставку может только создавший ее пользователь";
                case "gamble":
                    return "Принятие участия в текущей активной ставке\n" +
                "Использование:\ngamble [вариант исхода] [количество рейтинга, которое нужно поставить]\n" +
                "Повторное использование позволяет добавить рейтинг к ставке";
                case "current":
                    return "Получение текущей активной ставки";
                case "close":
                    return "Остановка принятия ставок";
                case "cancel":
                    return "Отмена текущей ставки без оглашения победившего исхода событий\n" +
                   "Закрыть ставку может только создавший ее пользователь";
                default:
                    return "Use arguments start/startcustom/result/gamble/current/close/cancel";
            }
        }
    }
}
