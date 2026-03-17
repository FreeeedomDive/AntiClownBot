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
                       PredictionsStartTime = new DateTime(2026, 03, 07),
                   },
                   new F1RaceInfo
                   {
                       Name = "Китай",
                       PredictionsStartTime = new DateTime(2026, 03, 14),
                   },
                   new F1RaceInfo
                   {
                       Name = "Япония",
                       PredictionsStartTime = new DateTime(2026, 03, 28),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Майами",
                       PredictionsStartTime = new DateTime(2026, 05, 02),
                   },
                   new F1RaceInfo
                   {
                       Name = "Канада",
                       PredictionsStartTime = new DateTime(2026, 05, 23),
                   },
                   new F1RaceInfo
                   {
                       Name = "Монако",
                       PredictionsStartTime = new DateTime(2026, 06, 06),
                   },
                   new F1RaceInfo
                   {
                       Name = "Испания Барселона",
                       PredictionsStartTime = new DateTime(2026, 06, 13),
                   },
                   new F1RaceInfo
                   {
                       Name = "Австрия",
                       PredictionsStartTime = new DateTime(2026, 06, 27),
                   },
                   new F1RaceInfo
                   {
                       Name = "Великобритания",
                       PredictionsStartTime = new DateTime(2026, 07, 04),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бельгия",
                       PredictionsStartTime = new DateTime(2026, 07, 18),
                   },
                   new F1RaceInfo
                   {
                       Name = "Венгрия",
                       PredictionsStartTime = new DateTime(2026, 07, 25),
                   },
                   new F1RaceInfo
                   {
                       Name = "Нидерланды",
                       PredictionsStartTime = new DateTime(2026, 08, 22),
                   },
                   new F1RaceInfo
                   {
                       Name = "Италия Монца",
                       PredictionsStartTime = new DateTime(2026, 09, 05),
                   },
                   new F1RaceInfo
                   {
                       Name = "Испания Мадрид",
                       PredictionsStartTime = new DateTime(2026, 09, 12),
                   },
                   new F1RaceInfo
                   {
                       Name = "Азербайджан",
                       PredictionsStartTime = new DateTime(2026, 09, 25),
                   },
                   new F1RaceInfo
                   {
                       Name = "Сингапур",
                       PredictionsStartTime = new DateTime(2026, 10, 10),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Остин",
                       PredictionsStartTime = new DateTime(2026, 10, 24),
                   },
                   new F1RaceInfo
                   {
                       Name = "Мексика",
                       PredictionsStartTime = new DateTime(2026, 10, 31),
                   },
                   new F1RaceInfo
                   {
                       Name = "Бразилия",
                       PredictionsStartTime = new DateTime(2026, 11, 07),
                   },
                   new F1RaceInfo
                   {
                       Name = "США Лас Вегас",
                       PredictionsStartTime = new DateTime(2026, 11, 20),
                   },
                   new F1RaceInfo
                   {
                       Name = "Катар",
                       PredictionsStartTime = new DateTime(2026, 11, 28),
                   },
                   new F1RaceInfo
                   {
                       Name = "Абу Даби",
                       PredictionsStartTime = new DateTime(2026, 12, 05),
                   },
               }
               .ToArray();
    }
}