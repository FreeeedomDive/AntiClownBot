using AntiClown.Api.Controllers;
using AntiClown.Data.Api.Controllers;
using AntiClown.DiscordBot.Web.Controllers;
using AntiClown.Entertainment.Api.Controllers.Parties;
using Xdd.HttpHelpers.HttpClientGenerator;

// Api
ApiClientGenerator.Generate<UsersController>(
    options => options.ProjectPath = Path.Join("..", "..", "..", "..", "AntiClown.Api.Client")
);

// EntertainmentApi
ApiClientGenerator.Generate<PartiesController>(
    options => options.ProjectPath = Path.Join("..", "..", "..", "..", "AntiClown.Entertainment.Api.Client")
);

// DataApi
ApiClientGenerator.Generate<TokensController>(
    options => options.ProjectPath = Path.Join("..", "..", "..", "..", "AntiClown.Data.Api.Client")
);

// DiscordApi
ApiClientGenerator.Generate<DiscordMembersController>(
    options => options.ProjectPath = Path.Join("..", "..", "..", "..", "AntiClown.DiscordBot.Client")
);