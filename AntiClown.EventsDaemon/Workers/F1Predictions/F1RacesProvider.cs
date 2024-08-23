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
                       Name = "Бахрейн",
                       PredictionsStartTime = new DateTime(2024, 02, 29),
                   },
                   new F1RaceInfo
                   {
                       Name = "Саудовская Аравия",
                       PredictionsStartTime = new DateTime(2024, 03, 07),
                   },
                   new F1RaceInfo
                   {
                       Name = "Австралия",
                       PredictionsStartTime = new DateTime(2024, 03, 22),
                   },
                   new F1RaceInfo
                   {
                       Name = "Япония",
                       PredictionsStartTime = new DateTime(2024, 04, 05),
                   },
                   new F1RaceInfo
                   {
                       Name = "Китай (спринт)",
                       PredictionsStartTime = new DateTime(2024, 04, 19),
                   },
                   new F1RaceInfo
                   {
                       Name = "Китай",
                       PredictionsStartTime = new DateTime(2024, 04, 19, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Майами (спринт)",
                       PredictionsStartTime = new DateTime(2024, 05, 03),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Майами",
                       PredictionsStartTime = new DateTime(2024, 05, 03, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Италия Имола",
                       PredictionsStartTime = new DateTime(2024, 05, 17),
                   },
                   new F1RaceInfo
                   {
                       Name = "Монако",
                       PredictionsStartTime = new DateTime(2024, 05, 24),
                   },
                   new F1RaceInfo
                   {
                       Name = "Канада",
                       PredictionsStartTime = new DateTime(2024, 06, 07),
                   },
                   new F1RaceInfo
                   {
                       Name = "Испания",
                       PredictionsStartTime = new DateTime(2024, 06, 22),
                   },
                   new F1RaceInfo
                   {
                       Name = "Австрия (спринт)",
                       PredictionsStartTime = new DateTime(2024, 06, 28),
                   },
                   new F1RaceInfo
                   {
                       Name = "Австрия",
                       PredictionsStartTime = new DateTime(2024, 06, 28, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Великобритания",
                       PredictionsStartTime = new DateTime(2024, 07, 05),
                   },
                   new F1RaceInfo
                   {
                       Name = "Венгрия",
                       PredictionsStartTime = new DateTime(2024, 07, 19),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бельгия",
                       PredictionsStartTime = new DateTime(2024, 07, 26),
                   },
                   new F1RaceInfo
                   {
                       Name = "Нидерланды",
                       PredictionsStartTime = new DateTime(2024, 08, 24),
                   },
                   new F1RaceInfo
                   {
                       Name = "Италия Монца",
                       PredictionsStartTime = new DateTime(2024, 08, 30),
                   },
                   new F1RaceInfo
                   {
                       Name = "Азербайджан",
                       PredictionsStartTime = new DateTime(2024, 09, 13),
                   },
                   new F1RaceInfo
                   {
                       Name = "Сингапур",
                       PredictionsStartTime = new DateTime(2024, 09, 20),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Остин (спринт)",
                       PredictionsStartTime = new DateTime(2024, 10, 18),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Остин",
                       PredictionsStartTime = new DateTime(2024, 10, 18, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Мексика",
                       PredictionsStartTime = new DateTime(2024, 10, 25),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бразилия (спринт)",
                       PredictionsStartTime = new DateTime(2024, 11, 01),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бразилия",
                       PredictionsStartTime = new DateTime(2024, 11, 01, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Лас Вегас",
                       PredictionsStartTime = new DateTime(2024, 11, 22),
                   },
                   new F1RaceInfo
                   {
                       Name = "Катар (спринт)",
                       PredictionsStartTime = new DateTime(2024, 11, 29),
                   },
                   new F1RaceInfo
                   {
                       Name = "Катар",
                       PredictionsStartTime = new DateTime(2024, 11, 29, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Абу Даби",
                       PredictionsStartTime = new DateTime(2024, 12, 06),
                   },
               }
               .Pipe(x => x.PredictionsStartTime = x.PredictionsStartTime.AddHours(7))
               .ToArray();
    }
}