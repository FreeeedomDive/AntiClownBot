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
                       PredictionsStartTime = new DateTime(2025, 03, 14, 5, 0, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Китай",
                       IsSprint = true,
                       PredictionsStartTime = new DateTime(2025, 03, 21),
                   },
                   new F1RaceInfo
                   {
                       Name = "Китай",
                       PredictionsStartTime = new DateTime(2025, 03, 21, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Япония",
                       PredictionsStartTime = new DateTime(2025, 04, 04),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бахрейн",
                       PredictionsStartTime = new DateTime(2025, 04, 11),
                   },
                   new F1RaceInfo
                   {
                       Name = "Саудовская Аравия",
                       PredictionsStartTime = new DateTime(2025, 04, 18),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Майами",
                       IsSprint = true,
                       PredictionsStartTime = new DateTime(2025, 05, 02),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Майами",
                       PredictionsStartTime = new DateTime(2025, 05, 02, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Италия Имола",
                       PredictionsStartTime = new DateTime(2025, 05, 16),
                   },
                   new F1RaceInfo
                   {
                       Name = "Монако",
                       PredictionsStartTime = new DateTime(2025, 05, 23),
                   },
                   new F1RaceInfo
                   {
                       Name = "Испания",
                       PredictionsStartTime = new DateTime(2025, 05, 30),
                   },
                   new F1RaceInfo
                   {
                       Name = "Канада",
                       PredictionsStartTime = new DateTime(2025, 06, 13),
                   },
                   new F1RaceInfo
                   {
                       Name = "Австрия",
                       PredictionsStartTime = new DateTime(2025, 06, 27),
                   },
                   new F1RaceInfo
                   {
                       Name = "Великобритания",
                       PredictionsStartTime = new DateTime(2025, 07, 04),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бельгия",
                       IsSprint = true,
                       PredictionsStartTime = new DateTime(2025, 07, 25),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бельгия",
                       PredictionsStartTime = new DateTime(2025, 07, 25, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Венгрия",
                       PredictionsStartTime = new DateTime(2025, 08, 01),
                   },
                   new F1RaceInfo
                   {
                       Name = "Нидерланды",
                       PredictionsStartTime = new DateTime(2025, 08, 29),
                   },
                   new F1RaceInfo
                   {
                       Name = "Италия Монца",
                       PredictionsStartTime = new DateTime(2025, 09, 05),
                   },
                   new F1RaceInfo
                   {
                       Name = "Азербайджан",
                       PredictionsStartTime = new DateTime(2025, 09, 19),
                   },
                   new F1RaceInfo
                   {
                       Name = "Сингапур",
                       PredictionsStartTime = new DateTime(2025, 10, 03),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Остин",
                       IsSprint = true,
                       PredictionsStartTime = new DateTime(2025, 10, 17),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Остин",
                       PredictionsStartTime = new DateTime(2025, 10, 17, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Мексика",
                       PredictionsStartTime = new DateTime(2025, 10, 24),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бразилия",
                       IsSprint = true,
                       PredictionsStartTime = new DateTime(2025, 11, 07),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бразилия",
                       PredictionsStartTime = new DateTime(2025, 11, 07, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Лас Вегас",
                       PredictionsStartTime = new DateTime(2025, 11, 20),
                   },
                   new F1RaceInfo
                   {
                       Name = "Катар",
                       IsSprint = true,
                       PredictionsStartTime = new DateTime(2025, 11, 28),
                   },
                   new F1RaceInfo
                   {
                       Name = "Катар",
                       PredictionsStartTime = new DateTime(2025, 11, 28, 0, 1, 0),
                   },
                   new F1RaceInfo
                   {
                       Name = "Абу Даби",
                       PredictionsStartTime = new DateTime(2025, 12, 05),
                   },
               }
               .Pipe(x => x.PredictionsStartTime = x.PredictionsStartTime.AddHours(7))
               .ToArray();
    }
}