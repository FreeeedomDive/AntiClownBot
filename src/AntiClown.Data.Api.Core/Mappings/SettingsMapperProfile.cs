using AntiClown.Data.Api.Core.SettingsStoring.Domain;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AutoMapper;

namespace AntiClown.Data.Api.Core.Mappings;

public class SettingsMapperProfile : Profile
{
    public SettingsMapperProfile()
    {
        CreateMap<SettingStorageElement, Setting>();
    }
}