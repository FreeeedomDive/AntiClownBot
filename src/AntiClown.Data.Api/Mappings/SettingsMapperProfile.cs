using AntiClown.Data.Api.Core.SettingsStoring.Domain;
using AntiClown.Data.Api.Dto.Settings;
using AutoMapper;

namespace AntiClown.Data.Api.Mappings;

public class SettingsMapperProfile : Profile
{
    public SettingsMapperProfile()
    {
        CreateMap<SettingDto, Setting>().ReverseMap();
    }
}