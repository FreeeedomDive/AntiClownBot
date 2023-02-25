using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Services;
using AntiClown.Api.Dto.Users;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/[controller]")]
public class UsersController : Controller
{
    public UsersController(
        IUsersService usersService,
        INewUserService newUserService,
        IMapper mapper
    )
    {
        this.usersService = usersService;
        this.newUserService = newUserService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<UserDto[]>> ReadAll()
    {
        var result = await usersService.ReadAllAsync();
        return mapper.Map<UserDto[]>(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> Read([FromRoute] Guid userId)
    {
        var result = await usersService.ReadAsync(userId);
        return mapper.Map<UserDto>(result);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<UserDto[]>> Find([FromBody] UserFilterDto filter)
    {
        var result = await usersService.FindAsync(mapper.Map<UserFilter>(filter));
        return mapper.Map<UserDto[]>(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] NewUserDto newUser)
    {
        return await newUserService.CreateNewUserAsync(mapper.Map<NewUser>(newUser));
    }

    private readonly IUsersService usersService;
    private readonly INewUserService newUserService;
    private readonly IMapper mapper;
}