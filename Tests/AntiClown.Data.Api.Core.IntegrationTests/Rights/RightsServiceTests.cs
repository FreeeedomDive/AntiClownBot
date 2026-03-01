using AutoFixture;
using FluentAssertions;
using DomainRights = AntiClown.Data.Api.Core.Rights.Domain.Rights;

namespace AntiClown.Data.Api.Core.IntegrationTests.Rights;

public class RightsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task GrantAsync_ShouldGrantRight_ToUser()
    {
        var userId = Fixture.Create<Guid>();

        await RightsService.GrantAsync(userId, DomainRights.EditSettings);

        var userRights = await RightsService.FindAllUserRightsAsync(userId);
        userRights.Should().Contain(DomainRights.EditSettings);
    }

    [Test]
    public async Task GrantAsync_ShouldBeIdempotent_WhenGrantedTwice()
    {
        var userId = Fixture.Create<Guid>();

        await RightsService.GrantAsync(userId, DomainRights.EditRights);
        await RightsService.GrantAsync(userId, DomainRights.EditRights);

        var userRights = await RightsService.FindAllUserRightsAsync(userId);
        userRights.Should().ContainSingle(r => r == DomainRights.EditRights);
    }

    [Test]
    public async Task RevokeAsync_ShouldRevokeRight()
    {
        var userId = Fixture.Create<Guid>();

        await RightsService.GrantAsync(userId, DomainRights.F1Predictions);
        await RightsService.RevokeAsync(userId, DomainRights.F1Predictions);

        var userRights = await RightsService.FindAllUserRightsAsync(userId);
        userRights.Should().NotContain(DomainRights.F1Predictions);
    }

    [Test]
    public async Task RevokeAsync_ShouldBeIdempotent_WhenRightNotGranted()
    {
        var userId = Fixture.Create<Guid>();

        await RightsService.Invoking(s => s.RevokeAsync(userId, DomainRights.F1PredictionsAdmin))
            .Should().NotThrowAsync();
    }

    [Test]
    public async Task FindAllUserRightsAsync_ShouldReturnAllGrantedRights()
    {
        var userId = Fixture.Create<Guid>();

        await RightsService.GrantAsync(userId, DomainRights.EditSettings);
        await RightsService.GrantAsync(userId, DomainRights.F1Predictions);

        var userRights = await RightsService.FindAllUserRightsAsync(userId);

        userRights.Should().Contain(DomainRights.EditSettings);
        userRights.Should().Contain(DomainRights.F1Predictions);
    }

    [Test]
    public async Task FindAllUserRightsAsync_ShouldReturnEmpty_ForNewUser()
    {
        var userId = Fixture.Create<Guid>();

        var userRights = await RightsService.FindAllUserRightsAsync(userId);

        userRights.Should().BeEmpty();
    }

    [Test]
    public async Task ReadAllAsync_ShouldGroupByRightType()
    {
        var userId1 = Fixture.Create<Guid>();
        var userId2 = Fixture.Create<Guid>();

        await RightsService.GrantAsync(userId1, DomainRights.EditSettings);
        await RightsService.GrantAsync(userId2, DomainRights.EditSettings);

        var allRights = await RightsService.ReadAllAsync();

        allRights.Should().ContainKey(DomainRights.EditSettings);
        allRights[DomainRights.EditSettings].Should().Contain(userId1);
        allRights[DomainRights.EditSettings].Should().Contain(userId2);
    }
}
