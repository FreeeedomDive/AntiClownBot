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
            Predictions = new[] { F1Driver.Tsunoda, F1Driver.Albon, F1Driver.Leclerc, F1Driver.Colapinto }.Select(
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
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                }
            ).ToList(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<F1Driver>(),
                SafetyCars = 0,
                FirstPlaceLead = 3m,
            },
        };
        var result = F1PredictionsResultBuilder.Build(race);
        CheckTenthPlacePrediction(race, F1Driver.Tsunoda, 15, result);
        CheckTenthPlacePrediction(race, F1Driver.Albon, 25, result);
        CheckTenthPlacePrediction(race, F1Driver.Leclerc, 6, result);
        CheckTenthPlacePrediction(race, F1Driver.Colapinto, 1, result);
    }

    private static void CheckTenthPlacePrediction(F1Race race, F1Driver tenthPlace, int expectedPoints, F1PredictionResult[] result)
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
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                },
                new()
                {
                    UserId = dnfPredictedUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = false,
                        DnfPickedDrivers = new[]
                        {
                            F1Driver.Albon,
                            F1Driver.Alonso,
                            F1Driver.Bottas,
                            F1Driver.Gasly,
                            F1Driver.Hamilton,
                        },
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                },
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<F1Driver>(),
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
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                },
                new()
                {
                    UserId = oneCorrectPredictionsUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = false,
                        DnfPickedDrivers = new[]
                        {
                            F1Driver.Albon,
                            F1Driver.Alonso,
                            F1Driver.Bottas,
                            F1Driver.Gasly,
                            F1Driver.Hamilton,
                        },
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                },
                new()
                {
                    UserId = threeCorrectPredictionsUserId,
                    RaceId = raceId,
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = false,
                        DnfPickedDrivers = new[]
                        {
                            F1Driver.Colapinto,
                            F1Driver.Alonso,
                            F1Driver.Stroll,
                            F1Driver.Gasly,
                            F1Driver.Hamilton,
                        },
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = F1SafetyCars.Zero,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                },
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = new[]
                {
                    F1Driver.Stroll,
                    F1Driver.Hamilton,
                    F1Driver.Colapinto,
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
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 5.0m,
                    SafetyCarsPrediction = x,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                }
            ).ToList(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<F1Driver>(),
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
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = x,
                    SafetyCarsPrediction = 0,
                    TeamsPickedDrivers = Array.Empty<F1Driver>(),
                }
            ).ToList(),
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<F1Driver>(),
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
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 3,
                    SafetyCarsPrediction = 0,
                    TeamsPickedDrivers = new[]
                    {
                        F1Driver.Perez,
                        F1Driver.Sainz,
                        F1Driver.Russell, // correct
                        F1Driver.Gasly, // correct
                        F1Driver.Norris, // correct
                        F1Driver.Zhou,
                        F1Driver.Alonso, // correct
                        F1Driver.Hulkenberg,
                        F1Driver.Tsunoda,
                        F1Driver.Colapinto,
                    },
                },
                new()
                {
                    UserId = userId2,
                    RaceId = raceId,
                    TenthPlacePickedDriver = F1Driver.Verstappen,
                    DnfPrediction = new F1DnfPrediction
                    {
                        NoDnfPredicted = true,
                    },
                    FirstPlaceLeadPrediction = 3,
                    SafetyCarsPrediction = 0,
                    TeamsPickedDrivers = new[]
                    {
                        F1Driver.Verstappen, // correct
                        F1Driver.Leclerc, // correct
                        F1Driver.Hamilton,
                        F1Driver.Doohan,
                        F1Driver.Piastri,
                        F1Driver.Bottas, // correct
                        F1Driver.Stroll,
                        F1Driver.Magnussen, // correct
                        F1Driver.Lawson, // correct
                        F1Driver.Albon, // correct
                    },
                },
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = Array.Empty<F1Driver>(),
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
                    F1Driver.Zhou,
                    F1Driver.Colapinto,
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
                TenthPlacePickedDriver = F1Driver.Russell, // 15 очков
                DnfPrediction = new F1DnfPrediction // 0 очков
                {
                    NoDnfPredicted = true,
                },
                SafetyCarsPrediction = F1SafetyCars.Two, // 5 очков
                FirstPlaceLeadPrediction = 7.455m, // 5 очков, так как он пока единственный и самый близкий к правильному ответу
                TeamsPickedDrivers = new[] // 4 очка
                {
                    F1Driver.Perez,
                    F1Driver.Sainz,
                    F1Driver.Russell, // correct
                    F1Driver.Gasly, // correct
                    F1Driver.Norris, // correct
                    F1Driver.Zhou,
                    F1Driver.Alonso, // correct
                    F1Driver.Hulkenberg,
                    F1Driver.Tsunoda,
                    F1Driver.Colapinto,
                },
            }
        );
        var results = F1PredictionsResultBuilder.Build(race);
        CountTotalPoints(results.First(x => x.UserId == userId1)).Should().Be(29);

        #endregion
        
        # region User2

        var userId2 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId2,
                TenthPlacePickedDriver = F1Driver.Doohan, // 4 очка
                DnfPrediction = new F1DnfPrediction     // 2 очка
                {
                    NoDnfPredicted = false,
                    DnfPickedDrivers = new[]
                    {
                        F1Driver.Verstappen,
                        F1Driver.Hamilton,
                        F1Driver.Stroll,
                        F1Driver.Colapinto,
                        F1Driver.Tsunoda,
                    },
                },
                SafetyCarsPrediction = F1SafetyCars.One, // 0 очков
                FirstPlaceLeadPrediction = 11.215m,      // 5 очков
                TeamsPickedDrivers = new[]               // 6 очков
                {
                    F1Driver.Verstappen, // correct
                    F1Driver.Leclerc,    // correct
                    F1Driver.Hamilton,
                    F1Driver.Doohan,
                    F1Driver.Piastri,
                    F1Driver.Bottas,     // correct
                    F1Driver.Stroll,
                    F1Driver.Magnussen,  // correct
                    F1Driver.Lawson,     // correct
                    F1Driver.Albon,      // correct
                },
            }
        );
        results = F1PredictionsResultBuilder.Build(race);
        CountTotalPoints(results.First(x => x.UserId == userId2)).Should().Be(17);

        #endregion
        
        # region User3

        var userId3 = Guid.NewGuid();
        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = raceId,
                UserId = userId3,
                TenthPlacePickedDriver = F1Driver.Albon, // 25 очков
                DnfPrediction = new F1DnfPrediction      // 4 очка
                {
                    NoDnfPredicted = false,
                    DnfPickedDrivers = new[]
                    {
                        F1Driver.Zhou,
                        F1Driver.Russell,
                        F1Driver.Alonso,
                        F1Driver.Colapinto,
                        F1Driver.Ricciardo,
                    },
                },
                SafetyCarsPrediction = F1SafetyCars.Zero, // 0 очков
                FirstPlaceLeadPrediction = 3.512m,        // 0 очков
                TeamsPickedDrivers = new[]                // 10 очков
                {
                    F1Driver.Verstappen, // correct
                    F1Driver.Leclerc,    // correct
                    F1Driver.Russell,    // correct
                    F1Driver.Gasly,      // correct
                    F1Driver.Norris,     // correct
                    F1Driver.Bottas,     // correct
                    F1Driver.Alonso,     // correct
                    F1Driver.Magnussen,  // correct
                    F1Driver.Lawson,     // correct
                    F1Driver.Albon,      // correct
                },
            }
        );
        results = F1PredictionsResultBuilder.Build(race);
        CountTotalPoints(results.First(x => x.UserId == userId3)).Should().Be(39);

        #endregion
    }

    private static int CountTotalPoints(F1PredictionResult result)
    {
        return result.TenthPlacePoints + result.DnfsPoints + result.SafetyCarsPoints + result.FirstPlaceLeadPoints + result.TeamMatesPoints;
    }

    private static readonly F1Driver[] TestClassification =
    {
        F1Driver.Norris, // 1
        F1Driver.Verstappen, // 2
        F1Driver.Piastri, // 4
        F1Driver.Leclerc, // 6
        F1Driver.Alonso, // 8
        F1Driver.Perez, // 10
        F1Driver.Sainz, // 12
        F1Driver.Russell, // 15
        F1Driver.Hamilton, // 18
        F1Driver.Albon, // 25
        F1Driver.Lawson, // 18
        F1Driver.Tsunoda, // 15
        F1Driver.Stroll, // 12
        F1Driver.Gasly, // 10
        F1Driver.Bottas, // 8
        F1Driver.Magnussen, // 6
        F1Driver.Doohan, // 4
        F1Driver.Hulkenberg, // 2
        F1Driver.Zhou, // 1
        F1Driver.Colapinto, // 1
    };
}