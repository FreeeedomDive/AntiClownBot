using AntiClownApi.AdminApi.SettingsServices.App;
using AntiClownApi.AdminApi.SettingsServices.Events;
using AntiClownApi.AdminApi.SettingsServices.Guild;
using AntiClownApiClient.Dto.Settings;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownApi.AdminApi.Controllers;

[ApiController]
[Route("/adminApi/settings")]
public class SettingsController : Controller
{
    public SettingsController(
        IAppSettingsService appSettingsService,
        IEventSettingsService eventSettingsService,
        IGuildSettingsService guildSettingsService
    )
    {
        this.appSettingsService = appSettingsService;
        this.eventSettingsService = eventSettingsService;
        this.guildSettingsService = guildSettingsService;
    }

    [HttpGet("application")]
    public ApplicationSettings ReadApplicationSettings()
    {
        return appSettingsService.GetSettings();
    }

    [HttpPost("application")]
    public void UpdateApplicationSettings([FromBody] ApplicationSettings newSettings)
    {
        appSettingsService.UpdateSettings(newSettings);
    }

    [HttpGet("events")]
    public EventSettings ReadEventsSettings()
    {
        return eventSettingsService.GetEventSettings();
    }

    [HttpPost("events")]
    public void UpdateEventsSettings([FromBody] EventSettings newSettings)
    {
        eventSettingsService.UpdateSettings(newSettings);
    }

    [HttpGet("guild")]
    public GuildSettings ReadGuildSettings()
    {
        return guildSettingsService.GetGuildSettings();
    }

    [HttpPost("guild")]
    public void UpdateGuildSettings([FromBody] GuildSettings newSettings)
    {
        guildSettingsService.UpdateSettings(newSettings);
    }

    private readonly IAppSettingsService appSettingsService;
    private readonly IEventSettingsService eventSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
}