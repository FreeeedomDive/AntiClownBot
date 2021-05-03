using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.F1.Quali
{
    public class F1QualiStartCommand : IF1QualiCommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public F1QualiStartCommand(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "start";

        public string Execute(SocialRatingUser user, string args)
        {
            if (Config.CurrentGamble != null)
            {
                return "В данный момент уже запущена ставка";
            }
            var lines = args.Split('\n');
            var gambleName = "F1 Quali P11-P15";
            var optionsLines = lines.Skip(1).ToList();
            if (optionsLines.Count != 20)
            {
                return "Гонщиков не 20, чел";
            }
            var options = new List<GambleOption>();
            var tuples = new Tuple<string, double>[20];
            foreach (var line in optionsLines)
            {
                var splittedline = line.Split(' ');
                if(splittedline.Length != 3)
                {
                    return "И что ты тут высрал?";
                }
                var positionParsed = int.TryParse(splittedline[0], out var position);
                if(!positionParsed)
                {
                    return $"Значение {splittedline[0]} не является типом int";
                }
                if(position < 1 || position > 20)
                {
                    return "Позиция не может быть меньше 1 или больше 20";
                }
                var option = splittedline[1];
                var isParsed = float.TryParse(splittedline.Last(), out var timeBehindFirst);
                if (!isParsed)
                {
                    return ($"Значение {splittedline.Last()} не является типом double");
                }
                if(tuples[position - 1] != null)
                {
                    return "Ало, у тебя 2 гонщика на 1 позиции";
                }
                tuples[position - 1] =  new Tuple<string, double>(option, timeBehindFirst);
            }
            if(tuples.Any(x => x == null))
            {
                return "Есть позиция без гонщика";
            }
            for(var count = 0; count < 20; count++)
            {
                if(count < 10)
                {
                    options.Add(new GambleOption(tuples[count].Item1, (tuples[10].Item2 - tuples[count].Item2) * (tuples[14].Item2 - tuples[count].Item2) * 10));
                }
                else if (count > 14)
                {
                    options.Add(new GambleOption(tuples[count].Item1, (tuples[count].Item2 - tuples[14].Item2) * (tuples[count].Item2 - tuples[14].Item2) * 10));
                }
                else
                {
                    options.Add(new GambleOption(tuples[count].Item1, 1.8));
                }
            }
            Config.CurrentGamble = new Gamble(gambleName, user.DiscordId, GambleType.WithCustomRatio, options);
            Config.Save();
            return $"Начата ставка \"{Config.CurrentGamble.GambleName}\"";
        }
    }
}
