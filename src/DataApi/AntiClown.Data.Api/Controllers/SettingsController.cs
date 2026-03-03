using AntiClown.Data.Api.Core.SettingsStoring.Services;
using AntiClown.Data.Api.Dto.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Data.Api.Controllers;

[Route("dataApi/settings")]
public class SettingsController(
    ISettingsService settingsService,
    IMapper mapper
) : Controller
{
    [HttpGet]
    public async Task<ActionResult<SettingDto[]>> ReadAllAsync()
    {
        var result = await settingsService.ReadAllAsync();
        return mapper.Map<SettingDto[]>(result);
    }

    [HttpGet("categories/{category}/keys/{key}")]
    public async Task<ActionResult<SettingDto>> ReadAsync([FromRoute] string category, [FromRoute] string key)
    {
        var result = await settingsService.ReadAsync(category, key);
        return mapper.Map<SettingDto>(result);
    }

    [HttpGet("categories/{category}")]
    public async Task<ActionResult<SettingDto[]>> FindAsync([FromRoute] string category)
    {
        var result = await settingsService.FindAsync(category);
        return mapper.Map<SettingDto[]>(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrUpdateAsync([FromBody] SettingDto settingDto)
    {
        await settingsService.CreateOrUpdateAsync(settingDto.Category, settingDto.Name, settingDto.Value);
        return NoContent();
    }
}