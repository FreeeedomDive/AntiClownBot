using AntiClown.Data.Api.Dto.Exceptions;
using AutoFixture;
using FluentAssertions;

namespace AntiClown.Data.Api.Core.IntegrationTests.Settings;

public class SettingsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task ReadAsync_ShouldThrowSettingNotFoundException_WhenSettingDoesNotExist()
    {
        var category = Fixture.Create<string>();
        var key = Fixture.Create<string>();

        await SettingsService.Invoking(s => s.ReadAsync(category, key))
            .Should().ThrowAsync<SettingNotFoundException>();
    }

    [Test]
    public async Task CreateOrUpdateAsync_ShouldCreateSetting()
    {
        var category = Fixture.Create<string>();
        var key = Fixture.Create<string>();
        var value = Fixture.Create<string>();

        await SettingsService.CreateOrUpdateAsync(category, key, value);

        var result = await SettingsService.ReadAsync(category, key);
        result.Category.Should().Be(category);
        result.Name.Should().Be(key);
        result.Value.Should().Be(value);
    }

    [Test]
    public async Task CreateOrUpdateAsync_ShouldUpdateExistingSetting()
    {
        var category = Fixture.Create<string>();
        var key = Fixture.Create<string>();
        var initialValue = Fixture.Create<string>();
        var updatedValue = Fixture.Create<string>();

        await SettingsService.CreateOrUpdateAsync(category, key, initialValue);
        await SettingsService.CreateOrUpdateAsync(category, key, updatedValue);

        var result = await SettingsService.ReadAsync(category, key);
        result.Value.Should().Be(updatedValue);
    }

    [Test]
    public async Task FindAsync_ShouldReturnSettingsByCategory()
    {
        var category = Fixture.Create<string>();
        var key1 = Fixture.Create<string>();
        var key2 = Fixture.Create<string>();
        var value1 = Fixture.Create<string>();
        var value2 = Fixture.Create<string>();

        await SettingsService.CreateOrUpdateAsync(category, key1, value1);
        await SettingsService.CreateOrUpdateAsync(category, key2, value2);

        var results = await SettingsService.FindAsync(category);

        results.Should().HaveCount(2);
        results.Should().Contain(s => s.Name == key1 && s.Value == value1);
        results.Should().Contain(s => s.Name == key2 && s.Value == value2);
    }

    [Test]
    public async Task ReadAllAsync_ShouldContainCreatedSettings()
    {
        var category = Fixture.Create<string>();
        var key = Fixture.Create<string>();
        var value = Fixture.Create<string>();

        await SettingsService.CreateOrUpdateAsync(category, key, value);

        var results = await SettingsService.ReadAllAsync();

        results.Should().Contain(s => s.Category == category && s.Name == key && s.Value == value);
    }
}
