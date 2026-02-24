using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.EventsDaemon.Workers.F1Predictions;

public class F1RacesProvider : IF1RacesProvider
{
    public F1RaceInfo[] GetRaces()
    {
        return new[]
               {
                   new F1RaceInfo
                   {
                       Name = "Австралия",
                       PredictionsStartTime = new DateTime(2026, 03, 06),
                   },
                   new F1RaceInfo
                   {
                       Name = "Китай",
                       PredictionsStartTime = new DateTime(2026, 03, 13),
                   },
                   new F1RaceInfo
                   {
                       Name = "Япония",
                       PredictionsStartTime = new DateTime(2026, 03, 27),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бахрейн",
                       PredictionsStartTime = new DateTime(2026, 04, 10),
                   },
                   new F1RaceInfo
                   {
                       Name = "Саудовская Аравия",
                       PredictionsStartTime = new DateTime(2026, 04, 17),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Майами",
                       PredictionsStartTime = new DateTime(2026, 05, 01),
                   },
                   new F1RaceInfo
                   {
                       Name = "Канада",
                       PredictionsStartTime = new DateTime(2026, 05, 22),
                   },
                   new F1RaceInfo
                   {
                       Name = "Монако",
                       PredictionsStartTime = new DateTime(2026, 06, 05),
                   },
                   new F1RaceInfo
                   {
                       Name = "Испания Барселона",
                       PredictionsStartTime = new DateTime(2026, 06, 12),
                   },
                   new F1RaceInfo
                   {
                       Name = "Австрия",
                       PredictionsStartTime = new DateTime(2026, 06, 26),
                   },
                   new F1RaceInfo
                   {
                       Name = "Великобритания",
                       PredictionsStartTime = new DateTime(2026, 07, 03),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бельгия",
                       PredictionsStartTime = new DateTime(2026, 07, 17),
                   },
                   new F1RaceInfo
                   {
                       Name = "Венгрия",
                       PredictionsStartTime = new DateTime(2026, 07, 24),
                   },
                   new F1RaceInfo
                   {
                       Name = "Нидерланды",
                       PredictionsStartTime = new DateTime(2026, 08, 21),
                   },
                   new F1RaceInfo
                   {
                       Name = "Италия Монца",
                       PredictionsStartTime = new DateTime(2026, 09, 04),
                   },
                   new F1RaceInfo
                   {
                       Name = "Испания Мадрид",
                       PredictionsStartTime = new DateTime(2026, 09, 11),
                   },
                   new F1RaceInfo
                   {
                       Name = "Азербайджан",
                       PredictionsStartTime = new DateTime(2026, 09, 24),
                   },
                   new F1RaceInfo
                   {
                       Name = "Сингапур",
                       PredictionsStartTime = new DateTime(2026, 10, 09),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Остин",
                       PredictionsStartTime = new DateTime(2026, 10, 23),
                   },
                   new F1RaceInfo
                   {
                       Name = "Мексика",
                       PredictionsStartTime = new DateTime(2026, 10, 30),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бразилия",
                       PredictionsStartTime = new DateTime(2026, 11, 06),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Лас Вегас",
                       PredictionsStartTime = new DateTime(2026, 11, 19),
                   },
                   new F1RaceInfo
                   {
                       Name = "Катар",
                       PredictionsStartTime = new DateTime(2026, 11, 27),
                   },
                   new F1RaceInfo
                   {
                       Name = "Абу Даби",
                       PredictionsStartTime = new DateTime(2026, 12, 04),
                   },
               }
               .Pipe(x => x.PredictionsStartTime = x.PredictionsStartTime.AddHours(7))
               .ToArray();
    }
}