using AutoFixture;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Economies;

public class TransactionsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task EconomyService_Should_CreateTransactions()
    {
        var currentTransactions = await TransactionsService.ReadManyAsync(User.Id);
        currentTransactions.Should().BeEmpty();
        var iterations = Fixture.Create<int>() % 20;
        for (var i = 1; i <= iterations; i++)
        {
            await EconomyService.UpdateScamCoinsAsync(User.Id, Fixture.Create<int>(), $"Тест транзакций {i}");
            currentTransactions = await TransactionsService.ReadManyAsync(User.Id);
            currentTransactions.Length.Should().Be(i);
        }

        currentTransactions.Should().BeInDescendingOrder(x => x.DateTime);
    }
}