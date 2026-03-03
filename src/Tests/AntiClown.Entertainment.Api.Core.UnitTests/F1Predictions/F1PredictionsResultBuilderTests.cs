using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.UnitTests.F1Predictions;

public class F1PredictionsResultBuilderTests
{
    [Test]
    public void EmptyPredictionsTest()
    {
        var race = CreateTestRace();
        var result = f1PredictionsResultBuilder.Build(race);
        result.Should().BeEmpty();
    }

    [TestCase("Norris", 1)]
    [TestCase("Piastri", 2)]
    [TestCase("Antonelli", 4)]
    [TestCase("Russell", 6)]
    [TestCase("Verstappen", 8)]
    [TestCase("Hadjar", 10)]
    [TestCase("Leclerc", 12)]
    [TestCase("Hamilton", 15)]
    [TestCase("Albon", 18)]
    [TestCase("Sainz", 25)]
    [TestCase("Lawson", 18)]
    [TestCase("Lindblad", 15)]
    [TestCase("Alonso", 12)]
    [TestCase("Stroll", 10)]
    [TestCase("Ocon", 8)]
    [TestCase("Bearman", 6)]
    [TestCase("Bortoleto", 4)]
    [TestCase("Hulkenberg", 2)]
    [TestCase("Gasly", 1)]
    [TestCase("Colapinto", 1)]
    [TestCase("Perez", 0)]
    [TestCase("Bottas", 0)]
    public void TenthPlacePointsTest(string selectedTenthDriver, int expectedPoints)
    {
        var race = CreateTestRace();

        race.Predictions.Add(CreatePrediction(race.Id, tenthPlacePickedDriver: selectedTenthDriver));

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.TenthPlacePoints.Should().Be(expectedPoints);
    }

    [TestCase("")]
    [TestCase("Norris,Leclerc,Stroll")]
    public void NoDnfPredictionsTest(string selectedDnfDrivers)
    {
        var race = CreateTestRace();
        var noDnfSelected = string.IsNullOrEmpty(selectedDnfDrivers);
        var dnfDrivers = selectedDnfDrivers.Split(',');

        race.Predictions.Add(CreatePrediction(race.Id, noDnfSelected: noDnfSelected, dnfDrivers: dnfDrivers));

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.DnfsPoints.Should().Be(noDnfSelected ? 10 : 0);
    }

    [TestCase("", 0)]
    [TestCase("Verstappen", 0)]
    [TestCase("Norris", 2)]
    [TestCase("Leclerc", 2)]
    [TestCase("Stroll", 2)]
    [TestCase("Albon", 2)]
    [TestCase("Hadjar", 2)]
    [TestCase("Norris,Leclerc", 4)]
    [TestCase("Norris,Leclerc,Stroll", 6)]
    [TestCase("Norris,Leclerc,Stroll,Albon", 8)]
    [TestCase("Leclerc,Stroll,Albon,Hadjar", 8)]
    [TestCase("Norris,Leclerc,Stroll,Albon,Hadjar", 10)]
    [TestCase("Norris,Leclerc,Stroll,Albon,Hadjar,Verstappen", 10)]
    public void DnfPredictionsTest(string selectedDnfDrivers, int expectedPoints)
    {
        var race = CreateTestRace();
        race.Result.DnfDrivers = ["Norris", "Leclerc", "Stroll", "Albon", "Hadjar"];
        var noDnfSelected = string.IsNullOrEmpty(selectedDnfDrivers);
        var dnfDrivers = selectedDnfDrivers.Split(',');

        race.Predictions.Add(CreatePrediction(race.Id, noDnfSelected: noDnfSelected, dnfDrivers: dnfDrivers));

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.DnfsPoints.Should().Be(expectedPoints);
    }

    [Test]
    public void SafetyCarsHelperTest()
    {
        F1PredictionsResultBuilder.ToSafetyCarsEnum(0).Should().Be(SafetyCarsCount.Zero);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(1).Should().Be(SafetyCarsCount.One);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(2).Should().Be(SafetyCarsCount.Two);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(3).Should().Be(SafetyCarsCount.ThreePlus);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(4).Should().Be(SafetyCarsCount.ThreePlus);
        F1PredictionsResultBuilder.ToSafetyCarsEnum(5).Should().Be(SafetyCarsCount.ThreePlus);

        Action negativeSafetyCarsCountAction = () => F1PredictionsResultBuilder.ToSafetyCarsEnum(-1);
        negativeSafetyCarsCountAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestCase(SafetyCarsCount.Zero, 0, 5)]
    [TestCase(SafetyCarsCount.Zero, 1, 0)]
    [TestCase(SafetyCarsCount.Zero, 2, 0)]
    [TestCase(SafetyCarsCount.Zero, 3, 0)]
    [TestCase(SafetyCarsCount.Zero, 4, 0)]
    [TestCase(SafetyCarsCount.One, 0, 0)]
    [TestCase(SafetyCarsCount.One, 1, 5)]
    [TestCase(SafetyCarsCount.One, 2, 0)]
    [TestCase(SafetyCarsCount.One, 3, 0)]
    [TestCase(SafetyCarsCount.One, 4, 0)]
    [TestCase(SafetyCarsCount.Two, 0, 0)]
    [TestCase(SafetyCarsCount.Two, 1, 0)]
    [TestCase(SafetyCarsCount.Two, 2, 5)]
    [TestCase(SafetyCarsCount.Two, 3, 0)]
    [TestCase(SafetyCarsCount.Two, 4, 0)]
    [TestCase(SafetyCarsCount.ThreePlus, 0, 0)]
    [TestCase(SafetyCarsCount.ThreePlus, 1, 0)]
    [TestCase(SafetyCarsCount.ThreePlus, 2, 0)]
    [TestCase(SafetyCarsCount.ThreePlus, 3, 5)]
    [TestCase(SafetyCarsCount.ThreePlus, 4, 5)]
    public void SafetyCarsPredictionsTest(SafetyCarsCount prediction, int actualIncidentsCount, int expectedPoints)
    {
        var race = CreateTestRace();
        race.Result.SafetyCars = actualIncidentsCount;

        race.Predictions.Add(CreatePrediction(race.Id, safetyCarsCount: prediction));

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.SafetyCarsPoints.Should().Be(expectedPoints);
    }

    [TestCase(1.5, 5, 0, 0, 0, 0)]
    [TestCase(2, 5, 0, 0, 0, 0)] /* первый поставил такой отрыв лидера раньше */
    [TestCase(3.5, 0, 5, 0, 0, 0)]
    [TestCase(5.5, 0, 0, 5, 0, 0)]
    [TestCase(7.5, 0, 0, 0, 5, 0)]
    [TestCase(9.5, 0, 0, 0, 0, 5)]
    public void FirstPlacePredictionTest(decimal actualFirstPlaceLead, int p1, int p2, int p3, int p4, int p5)
    {
        var race = CreateTestRace();
        race.Result.FirstPlaceLead = actualFirstPlaceLead;

        race.Predictions.AddRange(new[] { 1m, 3m, 5m, 7m, 9m }.Select(x => CreatePrediction(race.Id, firstPlaceLead: x)));

        var result = f1PredictionsResultBuilder.Build(race);
        result[0].FirstPlaceLeadPoints.Should().Be(p1);
        result[1].FirstPlaceLeadPoints.Should().Be(p2);
        result[2].FirstPlaceLeadPoints.Should().Be(p3);
        result[3].FirstPlaceLeadPoints.Should().Be(p4);
        result[4].FirstPlaceLeadPoints.Should().Be(p5);
    }

    [TestCase("Leclerc", 3, 0)]
    [TestCase("Leclerc", 4, 1)]
    [TestCase("Leclerc", 5, 4)]
    [TestCase("Leclerc", 6, 7)]
    [TestCase("Leclerc", 7, 10)]
    [TestCase("Leclerc", 8, 7)]
    [TestCase("Leclerc", 9, 4)]
    [TestCase("Leclerc", 10, 1)]
    [TestCase("Leclerc", 11, 0)]
    [TestCase("Norris", 1, 10)]
    [TestCase("Norris", 2, 7)]
    [TestCase("Norris", 3, 4)]
    [TestCase("Norris", 4, 1)]
    [TestCase("Norris", 5, 0)]
    [TestCase("Bottas", 22, 10)]
    [TestCase("Bottas", 21, 7)]
    [TestCase("Bottas", 20, 4)]
    [TestCase("Bottas", 19, 1)]
    [TestCase("Bottas", 18, 0)]
    public void SelectedDriverPositionPredictionTest(string driver, int prediction, int expectedPoints)
    {
        var race = CreateTestRace();
        race.Conditions!.PositionPredictionDriver = driver;

        race.Predictions.Add(CreatePrediction(race.Id, driverPosition: prediction));

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.DriverPositionPoints.Should().Be(expectedPoints);
    }

    // 15 + 0 + 5 + 5 + 7
    [TestCase("Hamilton", "", SafetyCarsCount.Two, 1, 8, 32)]
    // 8 + 0 + 0 + 5 + 1
    [TestCase("Verstappen", "Norris", SafetyCarsCount.One, 1, 4, 14)]
    // 0 + 2 + 0 + 5 + 1
    [TestCase("Perez", "Colapinto,Norris", SafetyCarsCount.ThreePlus, 1, 10, 8)]
    // 25 + 4 + 5 + 5 + 10
    [TestCase("Sainz", "Colapinto,Gasly", SafetyCarsCount.Two, 1, 7, 49)]
    public void ComplexPointsTest(
        string predictedTenthPoints,
        string predictedDnf,
        SafetyCarsCount predictedIncidents,
        decimal predictedFirstPlaceLead,
        int predictedDriverPosition,
        int expectedPoints
    )
    {
        var noDnfSelected = string.IsNullOrEmpty(predictedDnf);
        var dnfDrivers = predictedDnf.Split(',');
        var race = CreateTestRace();
        race.Result.DnfDrivers = ["Gasly", "Colapinto"];
        race.Result.SafetyCars = 2;

        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = race.Id,
                UserId = Guid.NewGuid(),
                TenthPlacePickedDriver = predictedTenthPoints,
                DnfPrediction = new DnfPrediction
                {
                    NoDnfPredicted = noDnfSelected,
                    DnfPickedDrivers = dnfDrivers,
                },
                SafetyCarsPrediction = predictedIncidents,
                FirstPlaceLeadPrediction = predictedFirstPlaceLead,
                DriverPositionPrediction = predictedDriverPosition,
            }
        );

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.TotalPoints.Should().Be(expectedPoints);
    }

    // 15 + 0 + 0 + 5 + 7
    [TestCase(2024, "Hamilton", "", SafetyCarsCount.Two, 1, 8, 27)]
    // 8 + 0 + 5 + 5 + 1
    [TestCase(2024, "Verstappen", "Norris", SafetyCarsCount.One, 1, 4, 19)]
    // 0 + 2 + 0 + 5 + 1
    [TestCase(2024, "Perez", "Colapinto,Norris", SafetyCarsCount.ThreePlus, 1, 10, 8)]
    // 25 + 4 + 0 + 5 + 10
    [TestCase(2024, "Sainz", "Colapinto,Gasly", SafetyCarsCount.Two, 1, 7, 44)]
    // (15 + 0 + 0 + 5 + 7)*0.3
    [TestCase(2025, "Hamilton", "", SafetyCarsCount.Two, 1, 8, 8)]
    // (8 + 0 + 5 + 5 + 1)*0.3
    [TestCase(2025, "Verstappen", "Norris", SafetyCarsCount.One, 1, 4, 5)]
    // (0 + 2 + 0 + 5 + 1)*0.3
    [TestCase(2025, "Perez", "Colapinto,Norris", SafetyCarsCount.ThreePlus, 1, 10, 2)]
    // (25 + 4 + 0 + 5 + 10)*0.3
    [TestCase(2025, "Sainz", "Colapinto,Gasly", SafetyCarsCount.Two, 1, 7, 13)]
    // 15 + 0 + 0 + 5 + 7
    [TestCase(2026, "Hamilton", "", SafetyCarsCount.Two, 1, 8, 27)]
    // 8 + 0 + 5 + 5 + 1
    [TestCase(2026, "Verstappen", "Norris", SafetyCarsCount.One, 1, 4, 19)]
    // 0 + 2 + 0 + 5 + 1
    [TestCase(2026, "Perez", "Colapinto,Norris", SafetyCarsCount.ThreePlus, 1, 10, 8)]
    // 25 + 4 + 0 + 5 + 10
    [TestCase(2026, "Sainz", "Colapinto,Gasly", SafetyCarsCount.Two, 1, 7, 44)]
    public void ComplexOldSeasonsSprintPointsTest(
        int season,
        string predictedTenthPoints,
        string predictedDnf,
        SafetyCarsCount predictedIncidents,
        decimal predictedFirstPlaceLead,
        int predictedDriverPosition,
        int expectedPoints
    )
    {
        var noDnfSelected = string.IsNullOrEmpty(predictedDnf);
        var dnfDrivers = predictedDnf.Split(',');
        var race = CreateTestRace(isSprint: true, season: season);
        race.Result.DnfDrivers = ["Gasly", "Colapinto"];
        race.Result.SafetyCars = 1;

        race.Predictions.Add(
            new F1Prediction
            {
                RaceId = race.Id,
                UserId = Guid.NewGuid(),
                TenthPlacePickedDriver = predictedTenthPoints,
                DnfPrediction = new DnfPrediction
                {
                    NoDnfPredicted = noDnfSelected,
                    DnfPickedDrivers = dnfDrivers,
                },
                SafetyCarsPrediction = predictedIncidents,
                FirstPlaceLeadPrediction = predictedFirstPlaceLead,
                DriverPositionPrediction = predictedDriverPosition,
            }
        );

        var result = f1PredictionsResultBuilder.Build(race).First();
        result.TotalPoints.Should().Be(expectedPoints);
    }

    private readonly IF1PredictionsResultBuilder f1PredictionsResultBuilder = new F1PredictionsResultBuilder();

    private static F1Race CreateTestRace(
        string? positionPredictionDriver = null,
        bool isSprint = false,
        int? season = null
    )
    {
        var raceId = Guid.NewGuid();
        return new F1Race
        {
            Id = raceId,
            Season = season ?? DateTime.UtcNow.Year,
            Name = "Гоночка",
            IsActive = true,
            IsOpened = true,
            IsSprint = isSprint,
            Conditions = new PredictionConditions
            {
                PositionPredictionDriver = positionPredictionDriver ?? TestClassification[6],
            },
            Predictions = [],
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = TestClassification,
                DnfDrivers = [],
                SafetyCars = 0,
                FirstPlaceLead = 7.069m,
            },
        };
    }

    private static F1Prediction CreatePrediction(
        Guid raceId,
        string tenthPlacePickedDriver = "Sainz",
        bool noDnfSelected = true,
        string[]? dnfDrivers = null,
        SafetyCarsCount safetyCarsCount = SafetyCarsCount.Zero,
        decimal firstPlaceLead = 1m,
        int driverPosition = 7
    )
    {
        return new F1Prediction
        {
            RaceId = raceId,
            UserId = Guid.NewGuid(),
            TenthPlacePickedDriver = tenthPlacePickedDriver,
            DnfPrediction = new DnfPrediction
            {
                NoDnfPredicted = noDnfSelected,
                DnfPickedDrivers = noDnfSelected ? [] : dnfDrivers ?? [],
            },
            SafetyCarsPrediction = safetyCarsCount,
            FirstPlaceLeadPrediction = firstPlaceLead,
            DriverPositionPrediction = driverPosition,
        };
    }

    private static readonly string[] TestClassification =
    [
        "Norris",
        "Piastri",
        "Antonelli",
        "Russell",
        "Verstappen",
        "Hadjar",
        "Leclerc",
        "Hamilton",
        "Albon",
        "Sainz",
        "Lawson",
        "Lindblad",
        "Alonso",
        "Stroll",
        "Ocon",
        "Bearman",
        "Bortoleto",
        "Hulkenberg",
        "Gasly",
        "Colapinto",
        "Perez",
        "Bottas",
    ];
}