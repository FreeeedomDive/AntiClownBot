using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Data.Api.Core.SettingsStoring.Repositories;

public class SettingsRepository : ISettingsRepository
{
    public SettingsRepository(ISqlRepository<SettingStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<SettingStorageElement[]> ReadAllAsync()
    {
        return await sqlRepository.ReadAllAsync();
    }

    public async Task<SettingStorageElement?> TryReadAsync(string category, string key)
    {
        var result = await sqlRepository.FindAsync(x => x.Category == category && x.Name == key);
        return result.FirstOrDefault();
    }

    public async Task<SettingStorageElement[]> FindAsync(string category)
    {
        return await sqlRepository.FindAsync(x => x.Category == category);
    }

    public async Task CreateOrUpdateAsync(string category, string key, string value)
    {
        var existing = await TryReadAsync(category, key);
        if (existing is not null)
        {
            await sqlRepository.UpdateAsync(existing.Id, x => x.Value = value);
            return;
        }

        await sqlRepository.CreateAsync(
            new SettingStorageElement
            {
                Id = Guid.NewGuid(),
                Category = category,
                Name = key,
                Value = value,
            }
        );
    }

    private readonly ISqlRepository<SettingStorageElement> sqlRepository;
}