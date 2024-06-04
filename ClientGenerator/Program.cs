using AntiClown.Api.Controllers;
using Xdd.HttpHelpers.HttpClientGenerator;

ApiClientGenerator.Generate<UsersController>(Path.Join("..", "..", "..", "..", "AntiClown.Api.Client"));