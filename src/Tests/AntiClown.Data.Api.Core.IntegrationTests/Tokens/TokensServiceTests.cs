using AutoFixture;
using FluentAssertions;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Data.Api.Core.IntegrationTests.Tokens;

public class TokensServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task GetAsync_ShouldGenerateNewToken_WhenNoTokenExists()
    {
        var userId = Fixture.Create<Guid>();

        var token = await TokensService.GetAsync(userId);

        token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task GetAsync_ShouldReturnSameToken_OnSubsequentCalls()
    {
        var userId = Fixture.Create<Guid>();

        var firstToken = await TokensService.GetAsync(userId);
        var secondToken = await TokensService.GetAsync(userId);

        secondToken.Should().Be(firstToken);
    }

    [Test]
    public async Task ValidateAsync_ShouldSucceed_WithCorrectToken()
    {
        var userId = Fixture.Create<Guid>();
        var token = await TokensService.GetAsync(userId);

        await TokensService.Invoking(s => s.ValidateAsync(userId, token))
            .Should().NotThrowAsync();
    }

    [Test]
    public async Task ValidateAsync_ShouldThrowUnauthorizedException_WithWrongToken()
    {
        var userId = Fixture.Create<Guid>();
        await TokensService.GetAsync(userId);

        await TokensService.Invoking(s => s.ValidateAsync(userId, "wrong-token"))
            .Should().ThrowAsync<UnauthorizedException>();
    }

    [Test]
    public async Task ValidateAsync_ShouldThrowUnauthorizedException_WhenNoTokenExists()
    {
        var userId = Fixture.Create<Guid>();

        await TokensService.Invoking(s => s.ValidateAsync(userId, "some-token"))
            .Should().ThrowAsync<UnauthorizedException>();
    }

    [Test]
    public async Task InvalidateAsync_ShouldRemoveToken_SoGetCreatesNewToken()
    {
        var userId = Fixture.Create<Guid>();
        var originalToken = await TokensService.GetAsync(userId);

        await TokensService.InvalidateAsync(userId);
        var newToken = await TokensService.GetAsync(userId);

        newToken.Should().NotBe(originalToken);
    }

    [Test]
    public async Task InvalidateAsync_ShouldBeIdempotent()
    {
        var userId = Fixture.Create<Guid>();
        await TokensService.GetAsync(userId);

        await TokensService.InvalidateAsync(userId);

        await TokensService.Invoking(s => s.InvalidateAsync(userId))
            .Should().NotThrowAsync();
    }
}
