using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;
using AutoMapper;

namespace AntiClown.Api.Core.Users.Services;

public class NewUserService : INewUserService
{
    public NewUserService(
        IUsersRepository usersRepository,
        IEconomyService economyService,
        IMapper mapper
    )
    {
        this.usersRepository = usersRepository;
        this.economyService = economyService;
        this.mapper = mapper;
    }

    public async Task<Guid> CreateNewUserAsync(NewUser newUser)
    {
        var user = mapper.Map<User>(newUser);
        await usersRepository.CreateAsync(user);
        // TODO: other things to create
        await economyService.CreateEmptyAsync(user.Id);
        return user.Id;
    }

    private readonly IUsersRepository usersRepository;
    private readonly IEconomyService economyService;
    private readonly IMapper mapper;
}