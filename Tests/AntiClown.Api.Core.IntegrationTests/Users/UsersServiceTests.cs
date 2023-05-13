using AntiClown.Api.Core.Users.Domain;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Users;

public class UsersServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task NewUserService_Should_CreateNewUser()
    {
        var discordId = CreateUniqueUlong();
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
    }
}