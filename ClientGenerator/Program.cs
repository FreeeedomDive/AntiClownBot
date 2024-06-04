using AntiClown.Api.Controllers;
using AntiClown.Data.Api.Controllers;
using AntiClown.DiscordBot.Web.Controllers;
using AntiClown.Entertainment.Api.Controllers.Parties;
using Xdd.HttpHelpers.HttpClientGenerator;

// Api
ApiClientGenerator.Generate<UsersController>(Path.Join("..", "..", "..", "..", "AntiClown.Api.Client"));

// EntertainmentApi
ApiClientGenerator.Generate<PartiesController>(Path.Join("..", "..", "..", "..", "AntiClown.Entertainment.Api.Client"));

// DataApi
ApiClientGenerator.Generate<TokensController>(Path.Join("..", "..", "..", "..", "AntiClown.Data.Api.Client"));

// DiscordApi
ApiClientGenerator.Generate<DiscordMembersController>(Path.Join("..", "..", "..", "..", "AntiClown.DiscordBot.Client"));