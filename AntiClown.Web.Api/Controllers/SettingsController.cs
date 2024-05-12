using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/settings")]
[RequireUserToken]
public class SettingsController : Controller
{
    public SettingsController(IAntiClownDataApiClient antiClownDataApiClient)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    [HttpGet("categories/{category}")]
    public async Task<ActionResult<SettingDto[]>> FindAsync([FromRoute] string category)
    {
        return await antiClownDataApiClient.Settings.FindAsync(category);
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrUpdateAsync([FromBody] SettingDto settingDto)
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(settingDto.Category, settingDto.Name, settingDto.Value);
        return NoContent();
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}