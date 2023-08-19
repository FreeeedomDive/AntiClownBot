using AntiClown.Core.Dto.Exceptions;
using AntiClown.Data.Api.Core.SettingsStoring.Domain;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Dto.Exceptions;
using AutoMapper;

namespace AntiClown.Data.Api.Core.SettingsStoring.Services;

public class SettingsService : ISettingsService
{
    public SettingsService(
        ISettingsRepository settingsRepository,
        IMapper mapper
    )
    {
        this.settingsRepository = settingsRepository;
        this.mapper = mapper;
    }

    public async Task<Setting[]> ReadAllAsync()
    {
        var result = await settingsRepository.ReadAllAsync();
        return mapper.Map<Setting[]>(result);
    }

    public async Task<Setting> ReadAsync(string category, string key)
    {
        var result = await settingsRepository.TryReadAsync(category, key);
        if (result is null)
        {
            throw new SettingNotFoundException(category, key);
        }
        return mapper.Map<Setting>(result);
    }

    public async Task<Setting[]> FindAsync(string category)
    {
        var result = await settingsRepository.FindAsync(category);
        return mapper.Map<Setting[]>(result);
    }

    public async Task CreateOrUpdateAsync(string category, string key, string value)
    {
        await settingsRepository.CreateOrUpdateAsync(category, key, value);
    }

    private readonly IMapper mapper;
    private readonly ISettingsRepository settingsRepository;
}