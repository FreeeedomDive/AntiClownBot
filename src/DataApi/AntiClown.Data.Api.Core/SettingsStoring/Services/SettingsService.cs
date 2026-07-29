using Xdd.HttpHelpers.Models.Exceptions;
using AntiClown.Data.Api.Core.SettingsStoring.Domain;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.SettingsStoring.Telemetry;
using AntiClown.Data.Api.Dto.Exceptions;
using AutoMapper;

namespace AntiClown.Data.Api.Core.SettingsStoring.Services;

public class SettingsService : ISettingsService
{
    public SettingsService(
        ISettingsRepository settingsRepository,
        IMapper mapper,
        SettingsTelemetry telemetry
    )
    {
        this.settingsRepository = settingsRepository;
        this.mapper = mapper;
        this.telemetry = telemetry;
    }

    public async Task<Setting[]> ReadAllAsync()
    {
        var result = await settingsRepository.ReadAllAsync();
        return mapper.Map<Setting[]>(result);
    }

    public async Task<Setting> ReadAsync(string category, string key)
    {
        var result = await settingsRepository.TryReadAsync(category, key);
        telemetry.RecordRead(result is not null);
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
    private readonly SettingsTelemetry telemetry;
}
