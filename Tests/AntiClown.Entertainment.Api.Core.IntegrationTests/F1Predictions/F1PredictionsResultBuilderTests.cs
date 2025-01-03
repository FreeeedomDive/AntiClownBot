using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Tools.Utility.Extensions;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1PredictionsResultBuilderTests
{
    [Test]
    public void EmptyPredictionsTest()
    {
        var race = new F1Race
        {
            Id = Guid.NewGuid(),
            Season = DateTime.UtcNow.Year,
            Name = "Тест без предсказаний",
            IsActive = true,
            IsOpened = true,
            Predictions = new List<F1Prediction>(),
            Result = new F1PredictionRaceResult(),
        };
        var result = F1PredictionsResultBuilder.Build(race);
        result.Should().BeEmpty();
    }

    [Test]
    public void TenthPlacePointsTest()
    {
        var raceId = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на подсчет очков за 10 место",
            IsActive = true,
            IsOpened = true,
            Predictions = new[] { "Tsunoda", "Albon", "Leclerc", "Colapinto" }.Select(
                x => new F1Prediction
                {
                    UserId = Guid.NewGuid(),
                    RaceId = raceId,
                    TenthPlacePickedDriver = x,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<string>(),
                }
            ).ToList(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<string>(),
                SafetyCars = 0,
                FirstPlaceLead = 3m,
            },
        };
        var result = F1PredictionsResultBuilder.Build(race);
        CheckTenthPlacePrediction(race, "Tsunoda", 15, result);
        CheckTenthPlacePrediction(race, "Albon", 25, result);
        CheckTenthPlacePrediction(race, "Leclerc", 6, result);
        CheckTenthPlacePrediction(race, "Colapinto", 1, result);
    }

    private static void CheckTenthPlacePrediction(F1Race race, string tenthPlace, int expectedPoints, F1PredictionResult[] result)
    {
        var predictedUserId = race.Predictions.First(x => x.TenthPlacePickedDriver == tenthPlace).UserId;
        result.First(x => x.UserId == predictedUserId).TenthPlacePoints.Should().Be(expectedPoints);
    }

    [Test]
    public void NoDnfPointsTest()
    {
        var raceId = Guid.NewGuid();
        var noDnfPredictUserId = Guid.NewGuid();
        var dnfPredictedUserId = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на очки за выбор NoDnf",
            IsActive = true,
            IsOpened = true,
            Predictions = new List<F1Prediction>
            {
                new()
                {
                    UserId = noDnfPredictUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<string>(),
                },
                new()
                {
                    UserId = dnfPredictedUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = false,
                        DnfPickedDrivers = new[]
                        {
                            "Albon",
                            "Alonso",
                            "Bottas",
                            "Gasly",
                            "Hamilton",
                        },
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<string>(),
                },
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<string>(),
                SafetyCars = 0,
                FirstPlaceLead = 3m,
            },
        };
        var result = F1PredictionsResultBuilder.Build(race);
        result.First(x => x.UserId == noDnfPredictUserId).DnfsPoints.Should().Be(10);
        result.First(x => x.UserId == dnfPredictedUserId).DnfsPoints.Should().Be(0);
    }

    [Test]
    public void DnfPointsTest()
    {
        var raceId = Guid.NewGuid();
        var noDnfPredictUserId = Guid.NewGuid();
        var oneCorrectPredictionsUserId = Guid.NewGuid();
        var threeCorrectPredictionsUserId = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на очки за выбор выбывших гонщиков",
            IsActive = true,
            IsOpened = true,
            Predictions = new List<F1Prediction>
            {
                new()
                {
                    UserId = noDnfPredictUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<string>(),
                },
                new()
                {
                    UserId = oneCorrectPredictionsUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = false,
                        DnfPickedDrivers = new[]
                        {
                            "Albon",
                            "Alonso",
                            "Bottas",
                            "Gasly",
                            "Hamilton",
                        },
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<string>(),
                },
                new()
                {
                    UserId = threeCorrectPredictionsUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = false,
                        DnfPickedDrivers = new[]
                        {
                            "Colapinto",
                            "Alonso",
                            "Stroll",
                            "Gasly",
                            "Hamilton",
                        },
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<string>(),
                },
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = new[]
                {
                    "Stroll",
                    "Hamilton",
                    "Colapinto",
                },
                SafetyCars = 0,
                FirstPlaceLead = 3m,
            },
        };
        var result = F1PredictionsResultBuilder.Build(race);
        result.First(x => x.UserId == noDnfPredictUserId).DnfsPoints.Should().Be(0);
        result.First(x => x.UserId == oneCorrectPredictionsUserId).DnfsPoints.Should().Be(2);
        result.First(x => x.UserId == threeCorrectPredictionsUserId).DnfsPoints.Should().Be(6);
    }

    [Test]
    public void SafetyCarsHelperTest()
    {
        F1PredictionsResultBuilder.ToSafetyCarsEnum(0).Should().Be(F1SafetyCars.Zero);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(1).Should().Be(F1SafetyCars.One);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(2).Should().Be(F1SafetyCars.Two);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(3).Should().Be(F1SafetyCars.ThreePlus);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(4).Should().Be(F1SafetyCars.ThreePlus);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(5).Should().Be(F1SafetyCars.ThreePlus);

        Action negativeSafetyCarsCountAction = () => F1PredictionsResultBuilder.ToSafetyCarsEnum(-1);
        negativeSafetyCarsCountAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void SafetyCarsPointsTest()
    {
        var raceId = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на подсчет очков за машины безопасности",
            IsActive = true,
            IsOpened = true,
            Predictions = new[] { F1SafetyCars.Zero, F1SafetyCars.One, F1SafetyCars.Two, F1SafetyCars.ThreePlus }.Select(
                x => new F1Prediction
                {
                    UserId = Guid.NewGuid(),
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = x,
                    TeamsPickedDrivers = Array.Empty<string>(),
                }
            ).ToList(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<string>(),
                SafetyCars = new Random().Next(0, 5),
                FirstPlaceLead = 3m,
            },
        };
        var expectedSafetyCarsResult = F1PredictionsResultBuilder.ToSafetyCarsEnum(race.Result.SafetyCars);
        var result = F1PredictionsResultBuilder.Build(race);
        var correctUserIds = race.Predictions.Where(x => x.SafetyCarsPrediction == expectedSafetyCarsResult).Select(x => x.UserId).ToArray();
        var correctResults = result.Where(x => correctUserIds.Contains(x.UserId)).ToArray();
        correctResults.ForEach(x => x.SafetyCarsPoints.Should().Be(5));
        var incorrectUserIds = race.Predictions.Where(x => x.SafetyCarsPrediction != expectedSafetyCarsResult).Select(x => x.UserId).ToArray();
        var incorrectResults = result.Where(x => incorrectUserIds.Contains(x.UserId)).ToArray();
        incorrectResults.ForEach(x => x.SafetyCarsPoints.Should().Be(0));
    }

    [Test]
    public void FirstPlaceLeadPointsTest()
    {
        var raceId = Guid.NewGuid();
        const decimal closestPrediction = 5.5m;
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на подсчет очков за отрыв 1 места",
            IsActive = true,
            IsOpened = true,
            Predictions = new[] { 1m, 3m, closestPrediction, 7m }.Select(
                x => new F1Prediction
                {
                    UserId = Guid.NewGuid(),
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = x,
                    SafetyCarsPrediction = 0,
                    TeamsPickedDrivers = Array.Empty<string>(),
                }
            ).ToList(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<string>(),
                SafetyCars = 2,
                FirstPlaceLead = 5m,
            },
        };
        var result = F1PredictionsResultBuilder.Build(race);
        var correctUserIds = race.Predictions.Where(x => x.FirstPlaceLeadPrediction == closestPrediction).Select(x => x.UserId).ToArray();
        var correctResults = result.Where(x => correctUserIds.Contains(x.UserId)).ToArray();
        correctResults.ForEach(x => x.FirstPlaceLeadPoints.Should().Be(5));
        var incorrectUserIds = race.Predictions.Where(x => x.FirstPlaceLeadPrediction != closestPrediction).Select(x => x.UserId).ToArray();
        var incorrectResults = result.Where(x => incorrectUserIds.Contains(x.UserId)).ToArray();
        incorrectResults.ForEach(x => x.FirstPlaceLeadPoints.Should().Be(0));
    }

    [Test]
    public void TeamMatesTest()
    {
        var raceId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на подсчет очков за определение победившего внутри команды гонщика",
            IsActive = true,
            IsOpened = true,
            Predictions = new List<F1Prediction>
            {
                new()
                {
                    UserId = userId1,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 3,
                    SafetyCarsPrediction = 0,
                    TeamsPickedDrivers = new[]
                    {
                        "Perez",
                        "Sainz",
                        "Russell", // correct
                        "Gasly", // correct
                        "Norris", // correct
                        "Zhou",
                        "Alonso", // correct
                        "Hulkenberg",
                        "Tsunoda",
                        "Colapinto",
                    },
                },
                new()
                {
                    UserId = userId2,
                    RaceId = raceId,
                    TenthPlacePickedDriver = "Verstappen",
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 3,
                    SafetyCarsPrediction = 0,
                    TeamsPickedDrivers = new[]
                    {
                        "Verstappen", // correct
                        "Leclerc", // correct
                        "Hamilton",
                        "Doohan",
                        "Piastri",
                        "Bottas", // correct
                        "Stroll",
                        "Magnussen", // correct
                        "Lawson", // correct
                        "Albon", // correct
                    },
                },
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<string>(),
                SafetyCars = 2,
                FirstPlaceLead = 5m,
            },
        };

        var result = F1PredictionsResultBuilder.Build(race);
        result.First(x => x.UserId == userId1).TeamMatesPoints.Should().Be(4);
        result.First(x => x.UserId == userId2).TeamMatesPoints.Should().Be(6);
    }

    [Test]
    public void ComplexPointsTest()
    {
        var raceId = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на подсчет очков за всю гонку",
            IsActive = true,
            IsOpened = true,
            Predictions = new List<F1Prediction>(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = new[]
                {
                    "Zhou",
                    "Colapinto",
                },
                SafetyCars = 2,
                FirstPlaceLead = 14.350m,
            },
        };

        # region User1

        var userId1 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId1,
                TenthPlacePickedDriver = "Russell", // 15 очков
                DnfPrediction = new F1DnfPrediction // 0 очков
                {
                    NoDnfPredicted = true,
                },
                SafetyCarsPrediction = F1SafetyCars.Two, // 5 очков
                FirstPlaceLeadPrediction = 7.455m, // 5 очков, так как он пока единственный и самый близкий к правильному ответу
                TeamsPickedDrivers = new[] // 4 очка
                {
                    "Perez",
                    "Sainz",
                    "Russell", // correct
                    "Gasly", // correct
                    "Norris", // correct
                    "Zhou",
                    "Alonso", // correct
                    "Hulkenberg",
                    "Tsunoda",
                    "Colapinto",
                },
            }
        );
        var results = F1PredictionsResultBuilder.Build(race);
        results.First(x => x.UserId == userId1).TotalPoints.Should().Be(29);

        #endregion
        
        # region User2

        var userId2 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId2,
                TenthPlacePickedDriver = "Doohan", // 4 очка
                DnfPrediction = new F1DnfPrediction     // 2 очка
                {
                    NoDnfPredicted = false,
                    DnfPickedDrivers = new[]
                    {
                        "Verstappen",
                        "Hamilton",
                        "Stroll",
                        "Colapinto",
                        "Tsunoda",
                    },
                },
                SafetyCarsPrediction = F1SafetyCars.One, // 0 очков
                FirstPlaceLeadPrediction = 11.215m,      // 5 очков
                TeamsPickedDrivers = new[]               // 6 очков
                {
                    "Verstappen", // correct
                    "Leclerc",    // correct
                    "Hamilton",
                    "Doohan",
                    "Piastri",
                    "Bottas",     // correct
                    "Stroll",
                    "Magnussen",  // correct
                    "Lawson",     // correct
                    "Albon",      // correct
                },
            }
        );
        results = F1PredictionsResultBuilder.Build(race);
        results.First(x => x.UserId == userId2).TotalPoints.Should().Be(17);
        // второй челик теперь ближе в предикте первого места, у первого -5 за это предсказание 
        results.First(x => x.UserId == userId1).TotalPoints.Should().Be(24);

        #endregion
        
        # region User3

        var userId3 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId3,
                TenthPlacePickedDriver = "Albon", // 25 очков
                DnfPrediction = new F1DnfPrediction      // 4 очка
                {
                    NoDnfPredicted = false,
                    DnfPickedDrivers = new[]
                    {
                        "Zhou",
                        "Russell",
                        "Alonso",
                        "Colapinto",
                        "Ricciardo",
                    },
                },
                SafetyCarsPrediction = F1SafetyCars.Zero, // 0 очков
                FirstPlaceLeadPrediction = 3.512m,        // 0 очков
                TeamsPickedDrivers = new[]                // 10 очков
                {
                    "Verstappen", // correct
                    "Leclerc",    // correct
                    "Russell",    // correct
                    "Gasly",      // correct
                    "Norris",     // correct
                    "Bottas",     // correct
                    "Alonso",     // correct
                    "Magnussen",  // correct
                    "Lawson",     // correct
                    "Albon",      // correct
                },
            }
        );
        results = F1PredictionsResultBuilder.Build(race);
        results.First(x => x.UserId == userId3).TotalPoints.Should().Be(39);

        #endregion
    }

    [Test]
    public void ComplexPointsSprintTest()
    {
        var raceId = Guid.NewGuid();
        var race = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = "Тест на подсчет очков за всю гонку",
            IsActive = true,
            IsSprint = true,
            IsOpened = true,
            Predictions = new List<F1Prediction>(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = new[]
                {
                    "Zhou",
                    "Colapinto",
                },
                SafetyCars = 2,
                FirstPlaceLead = 14.350m,
            },
        };

        # region User1

        var userId1 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId1,
                TenthPlacePickedDriver = "Russell", // 15 очков
                DnfPrediction = new F1DnfPrediction // 0 очков
                {
                    NoDnfPredicted = true,
                },
                SafetyCarsPrediction = F1SafetyCars.Two, // 5 очков
                FirstPlaceLeadPrediction = 7.455m, // 5 очков, так как он пока единственный и самый близкий к правильному ответу
                TeamsPickedDrivers = new[] // 4 очка
                {
                    "Perez",
                    "Sainz",
                    "Russell", // correct
                    "Gasly", // correct
                    "Norris", // correct
                    "Zhou",
                    "Alonso", // correct
                    "Hulkenberg",
                    "Tsunoda",
                    "Colapinto",
                },
            }
        );
        var results = F1PredictionsResultBuilder.Build(race);
        results.First(x => x.UserId == userId1).TotalPoints.Should().Be(8);

        #endregion
        
        # region User2

        var userId2 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId2,
                TenthPlacePickedDriver = "Doohan", // 4 очка
                DnfPrediction = new F1DnfPrediction     // 2 очка
                {
                    NoDnfPredicted = false,
                    DnfPickedDrivers = new[]
                    {
                        "Verstappen",
                        "Hamilton",
                        "Stroll",
                        "Colapinto",
                        "Tsunoda",
                    },
                },
                SafetyCarsPrediction = F1SafetyCars.One, // 0 очков
                FirstPlaceLeadPrediction = 11.215m,      // 5 очков
                TeamsPickedDrivers = new[]               // 6 очков
                {
                    "Verstappen", // correct
                    "Leclerc",    // correct
                    "Hamilton",
                    "Doohan",
                    "Piastri",
                    "Bottas",     // correct
                    "Stroll",
                    "Magnussen",  // correct
                    "Lawson",     // correct
                    "Albon",      // correct
                },
            }
        );
        results = F1PredictionsResultBuilder.Build(race);
        results.First(x => x.UserId == userId2).TotalPoints.Should().Be(5);
        // второй челик теперь ближе в предикте первого места, у первого -5 за это предсказание 
        results.First(x => x.UserId == userId1).TotalPoints.Should().Be(7);

        #endregion
        
        # region User3

        var userId3 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId3,
                TenthPlacePickedDriver = "Albon", // 25 очков
                DnfPrediction = new F1DnfPrediction      // 4 очка
                {
                    NoDnfPredicted = false,
                    DnfPickedDrivers = new[]
                    {
                        "Zhou",
                        "Russell",
                        "Alonso",
                        "Colapinto",
                        "Ricciardo",
                    },
                },
                SafetyCarsPrediction = F1SafetyCars.Zero, // 0 очков
                FirstPlaceLeadPrediction = 3.512m,        // 0 очков
                TeamsPickedDrivers = new[]                // 10 очков
                {
                    "Verstappen", // correct
                    "Leclerc",    // correct
                    "Russell",    // correct
                    "Gasly",      // correct
                    "Norris",     // correct
                    "Bottas",     // correct
                    "Alonso",     // correct
                    "Magnussen",  // correct
                    "Lawson",     // correct
                    "Albon",      // correct
                },
            }
        );
        results = F1PredictionsResultBuilder.Build(race);
        results.First(x => x.UserId == userId3).TotalPoints.Should().Be(11);

        #endregion
    }

    private static readonly string[] TestClassification =
    {
        "Norris", // 1
        "Verstappen", // 2
        "Piastri", // 4
        "Leclerc", // 6
        "Alonso", // 8
        "Perez", // 10
        "Sainz", // 12
        "Russell", // 15
        "Hamilton", // 18
        "Albon", // 25
        "Lawson", // 18
        "Tsunoda", // 15
        "Stroll", // 12
        "Gasly", // 10
        "Bottas", // 8
        "Magnussen", // 6
        "Doohan", // 4
        "Hulkenberg", // 2
        "Zhou", // 1
        "Colapinto", // 1
    };
}