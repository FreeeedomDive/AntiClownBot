using AntiClown.Api.Core.Achievements.Domain;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Achievements;

public class UserAchievementsTests : IntegrationTestsBase
{
    [SetUp]
    public async Task AchievementsSetUp()
    {
        achievement = await AchievementsService.ReadAsync(
            await AchievementsService.CreateAsync(
                new NewAchievement
                {
                    Name = "Test Achievement",
                    Logo = new byte[] { 1, 2, 3 },
                }
            )
        );
    }

    [Test]
    public async Task AchievementsService_Grant_Should_AppearInGetByUserId()
    {
        await AchievementsService.GrantAsync(achievement.Id, User.Id, DateTime.UtcNow);

        var userAchievements = await AchievementsService.GetByUserIdAsync(User.Id);
        userAchievements.Should().Contain(ua => ua.AchievementId == achievement.Id && ua.UserId == User.Id);
    }

    [Test]
    public async Task AchievementsService_Grant_Should_AppearInGetByAchievementId()
    {
        await AchievementsService.GrantAsync(achievement.Id, User.Id, DateTime.UtcNow);

        var grantedUsers = await AchievementsService.GetByAchievementIdAsync(achievement.Id);
        grantedUsers.Should().Contain(ua => ua.UserId == User.Id && ua.AchievementId == achievement.Id);
    }

    [Test]
    public async Task AchievementsService_Grant_Should_PersistProvidedGrantedAt()
    {
        var grantedAt = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        await AchievementsService.GrantAsync(achievement.Id, User.Id, grantedAt);

        var userAchievements = await AchievementsService.GetByUserIdAsync(User.Id);
        var granted = userAchievements.Single(ua => ua.AchievementId == achievement.Id);
        granted.GrantedAt.Should().Be(grantedAt);
    }

    [Test]
    public async Task AchievementsService_Grant_SameAchievement_ToMultipleUsers()
    {
        var secondUserId = await NewUserService.CreateNewUserAsync(
            new AntiClown.Api.Core.Users.Domain.NewUser
            {
                DiscordId = CreateUniqueUlong(),
            }
        );

        await AchievementsService.GrantAsync(achievement.Id, User.Id, DateTime.UtcNow);
        await AchievementsService.GrantAsync(achievement.Id, secondUserId, DateTime.UtcNow);

        var grantedUsers = await AchievementsService.GetByAchievementIdAsync(achievement.Id);
        grantedUsers.Should().Contain(ua => ua.UserId == User.Id);
        grantedUsers.Should().Contain(ua => ua.UserId == secondUserId);
    }

    private Achievement achievement = null!;
}
