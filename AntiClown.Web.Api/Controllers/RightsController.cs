﻿using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Rights;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/rights")]
[RequireUserToken]
public class RightsController : Controller
{
    public RightsController(IAntiClownDataApiClient antiClownDataApiClient)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<RightsDto[]>> FindAllUserRights([FromRoute] Guid userId)
    {
        return await antiClownDataApiClient.Rights.FindAllUserRightsAsync(userId);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}