using AntiClown.Api.Core.Achievements.Domain;
using AntiClown.Core.Dto.Exceptions;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Achievements;

public class AchievementsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task AchievementsService_Should_CreateAndReadBack()
    {
        var logoBytes = new byte[] { 1, 2, 3, 4, 5 };
        var id = await AchievementsService.CreateAsync(
            new NewAchievement
            {
                Name = "Test Achievement",
                Logo = logoBytes,
            }
        );

        var achievement = await AchievementsService.ReadAsync(id);
        achievement.Id.Should().Be(id);
        achievement.Name.Should().Be("Test Achievement");
        achievement.Logo.Should().BeEquivalentTo(logoBytes);
    }

    [Test]
    public async Task AchievementsService_ReadAll_Should_ContainCreated()
    {
        var id = await AchievementsService.CreateAsync(
            new NewAchievement
            {
                Name = "ReadAll Test Achievement",
                Logo = new byte[] { 10, 20, 30 },
            }
        );

        var all = await AchievementsService.ReadAllAsync();
        all.Should().Contain(a => a.Id == id);
    }

    [Test]
    public async Task AchievementsService_Delete_Should_RemoveAchievement()
    {
        var id = await AchievementsService.CreateAsync(
            new NewAchievement
            {
                Name = "Delete Test Achievement",
                Logo = new byte[] { 7, 8, 9 },
            }
        );

        await AchievementsService.DeleteAsync(id);

        var readAfterDelete = () => AchievementsService.ReadAsync(id);
        await readAfterDelete.Should().ThrowAsync<EntityNotFoundException>();
    }
}
