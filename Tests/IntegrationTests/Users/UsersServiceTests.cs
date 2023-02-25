using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Users.Domain;
using AutoFixture;
using FluentAssertions;

namespace IntegrationTests.Users;

public class UsersServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task NewUserService_Should_CreateNewUser()
    {
        var discordId = Fixture.Create<ulong>();
        var current = await UsersService.FindAsync(new UserFilter
        {
            DiscordId = discordId
        });
        current.Should().BeEmpty();
        var newUserId = await NewUserService.CreateNewUserAsync(new NewUser
        {
            DiscordId = discordId
        });
        var readNewUserFunc = () => UsersService.ReadAsync(newUserId);
        await readNewUserFunc.Should().NotThrowAsync();
        var newUser = await readNewUserFunc();
        newUser.DiscordId.Should().Be(discordId);
        var newUserEconomy = await EconomyService.ReadEconomyAsync(newUserId);
        newUserEconomy.Id.Should().Be(newUserId);
        newUserEconomy.Should().BeEquivalentTo(Economy.Default);
    }
}