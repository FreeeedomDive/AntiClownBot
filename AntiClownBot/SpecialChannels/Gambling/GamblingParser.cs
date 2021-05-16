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
                return;
            }
            var message = command.Execute(e, user);
            await e.Message.RespondAsync(message);
            Config.Save();
        }
        public override string Help(MessageCreateEventArgs e)
        {
            return e.Message.Content.Split(' ').Skip(1).FirstOrDefault() switch
            {
                "start" => "Начало новой ставки\nМожно иметь только одну активную ставку\nИспользование:\n" +
                           "start [Название ставки]\n[Вариант 1]\n[Вариант 2]\n...\n[Вариант N]",
                "startcustom" => "Начало новой ставки с кастомными коэффициентами на каждый вариант\n" +
                                 "Можно иметь только одну активную ставку\nИспользование:\n" +
                                 "startcustom [Название ставки]\n[Вариант 1 [Коэффициент1]]\n[Вариант 2 [Коэффициент2]]\n..." +
                                 "\n[Вариант N [КоэффициентN]]",
                "result" =>
                    "Закрытие текущей ставки с выбором победившего исхода событий\nИспользование:\nresult\n[Исход1]\n...\n[Исход N]\n" +
                    "Закрыть ставку может только создавший ее пользователь",
                "gamble" => "Принятие участия в текущей активной ставке\n" +
                            "Использование:\ngamble [вариант исхода] [количество рейтинга, которое нужно поставить]\n" +
                            "Повторное использование позволяет добавить рейтинг к ставке",
                "current" => "Получение текущей активной ставки",
                "close" => "Остановка принятия ставок",
                "cancel" => "Отмена текущей ставки без оглашения победившего исхода событий\n" +
                            "Закрыть ставку может только создавший ее пользователь",
                _ => "Аргументы: start/startcustom/result/gamble/current/close/cancel"
            };
        }
    }
}
