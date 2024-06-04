using AntiClown.Api.Controllers;
using AntiClown.Entertainment.Api.Controllers.Parties;
using Xdd.HttpHelpers.HttpClientGenerator;

ApiClientGenerator.Generate<UsersController>(Path.Join("..", "..", "..", "..", "AntiClown.Api.Client"));
ApiClientGenerator.Generate<PartiesController>(Path.Join("..", "..", "..", "..", "AntiClown.Entertainment.Api.Client"));