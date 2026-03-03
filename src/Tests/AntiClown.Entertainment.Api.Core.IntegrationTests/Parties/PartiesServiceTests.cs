using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AutoFixture;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.Parties;

public class PartiesServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task CreateAsync_Should_CreateOpenParty()
    {
        var newParty = CreatePartyRequest();

        var id = await PartiesService.CreateAsync(newParty);

        var party = await PartiesService.ReadAsync(id);
        party.Should().NotBeNull();
        party.Name.Should().Be(newParty.Name);
        party.Description.Should().Be(newParty.Description);
        party.MaxPlayers.Should().Be(newParty.MaxPlayers);
        party.CreatorId.Should().Be(newParty.CreatorId);
        party.IsOpened.Should().BeTrue();
        party.FirstFullPartyAt.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_WithAutoJoin_Should_AddCreatorToParticipants()
    {
        var creatorId = Guid.NewGuid();
        var newParty = CreatePartyRequest(creatorId: creatorId, authorAutoJoin: true);

        var id = await PartiesService.CreateAsync(newParty);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().Contain(creatorId);
    }

    [Test]
    public async Task CreateAsync_WithoutAutoJoin_Should_NotAddCreatorToParticipants()
    {
        var creatorId = Guid.NewGuid();
        var newParty = CreatePartyRequest(creatorId: creatorId, authorAutoJoin: false);

        var id = await PartiesService.CreateAsync(newParty);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().BeEmpty();
    }

    [Test]
    public async Task ReadOpenedAsync_Should_ContainNewlyCreatedParty()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest());

        var openedParties = await PartiesService.ReadOpenedAsync();

        openedParties.Should().Contain(p => p.Id == id);
    }

    [Test]
    public async Task ReadOpenedAsync_Should_NotContainClosedParty()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest());
        await PartiesService.CloseAsync(id);

        var openedParties = await PartiesService.ReadOpenedAsync();

        openedParties.Should().NotContain(p => p.Id == id);
    }

    [Test]
    public async Task AddPlayerAsync_Should_AddPlayerToParticipants()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        var userId = Guid.NewGuid();

        await PartiesService.AddPlayerAsync(id, userId);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().Contain(userId);
    }

    [Test]
    public async Task AddPlayerAsync_Should_BeIdempotent_WhenPlayerAlreadyInParty()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        var userId = Guid.NewGuid();

        await PartiesService.AddPlayerAsync(id, userId);
        await PartiesService.AddPlayerAsync(id, userId);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().ContainSingle(p => p == userId);
    }

    [Test]
    public async Task AddPlayerAsync_Should_SetFirstFullPartyAt_WhenPartyBecomesFullForFirstTime()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 2));

        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());
        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());

        var party = await PartiesService.ReadAsync(id);
        party.FirstFullPartyAt.Should().NotBeNull();
    }

    [Test]
    public async Task AddPlayerAsync_Should_NotChangeFirstFullPartyAt_WhenAlreadySet()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 2));

        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());
        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());
        var partyAfterFull = await PartiesService.ReadAsync(id);
        var firstFullTime = partyAfterFull.FirstFullPartyAt;

        // Remove and re-add to reach full capacity again
        var extraUserId = Guid.NewGuid();
        await PartiesService.AddPlayerAsync(id, extraUserId);

        var party = await PartiesService.ReadAsync(id);
        party.FirstFullPartyAt.Should().Be(firstFullTime);
    }

    [Test]
    public async Task AddPlayerAsync_Should_DoNothing_WhenPartyIsClosed()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        await PartiesService.CloseAsync(id);
        var userId = Guid.NewGuid();

        await PartiesService.AddPlayerAsync(id, userId);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().NotContain(userId);
    }

    [Test]
    public async Task RemovePlayerAsync_Should_RemovePlayerFromParticipants()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        var userId = Guid.NewGuid();
        await PartiesService.AddPlayerAsync(id, userId);

        await PartiesService.RemovePlayerAsync(id, userId);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().NotContain(userId);
    }

    [Test]
    public async Task RemovePlayerAsync_Should_DoNothing_WhenPlayerNotInParty()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        var otherUserId = Guid.NewGuid();
        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());

        await PartiesService.RemovePlayerAsync(id, otherUserId);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().HaveCount(1);
    }

    [Test]
    public async Task RemovePlayerAsync_Should_DoNothing_WhenPartyIsClosed()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        var userId = Guid.NewGuid();
        await PartiesService.AddPlayerAsync(id, userId);
        await PartiesService.CloseAsync(id);

        await PartiesService.RemovePlayerAsync(id, userId);

        var party = await PartiesService.ReadAsync(id);
        party.Participants.Should().Contain(userId);
    }

    [Test]
    public async Task CloseAsync_Should_CloseParty()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest());

        await PartiesService.CloseAsync(id);

        var party = await PartiesService.ReadAsync(id);
        party.IsOpened.Should().BeFalse();
    }

    [Test]
    public async Task CloseAsync_Should_BeIdempotent_WhenPartyAlreadyClosed()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest());
        await PartiesService.CloseAsync(id);

        var act = () => PartiesService.CloseAsync(id);

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task ReadFullPartiesAsync_Should_ReturnFullParties()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 2));

        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());
        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());

        var fullParties = await PartiesService.ReadFullPartiesAsync();

        fullParties.Should().Contain(p => p.Id == id);
    }

    [Test]
    public async Task ReadFullPartiesAsync_Should_NotReturnNonFullParties()
    {
        var id = await PartiesService.CreateAsync(CreatePartyRequest(maxPlayers: 5));
        await PartiesService.AddPlayerAsync(id, Guid.NewGuid());

        var fullParties = await PartiesService.ReadFullPartiesAsync();

        fullParties.Should().NotContain(p => p.Id == id);
    }

    private CreateParty CreatePartyRequest(
        Guid? creatorId = null,
        int maxPlayers = 4,
        bool authorAutoJoin = false)
    {
        return new CreateParty
        {
            Name = $"Test Party {Guid.NewGuid()}",
            Description = "Integration test party",
            MaxPlayers = maxPlayers,
            CreatorId = creatorId ?? Guid.NewGuid(),
            RoleId = (ulong)Fixture.Create<long>(),
            AuthorAutoJoin = authorAutoJoin,
        };
    }
}
