using AntiClown.DiscordBot.Database;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigurePostgreSql<DatabaseContext>(builder.Configuration.GetSection("PostgreSql"));

var app = builder.Build();

var databaseContextFactory = app.Services.GetRequiredService<IDbContextFactory<DatabaseContext>>();
var databaseContext = await databaseContextFactory.CreateDbContextAsync();
await databaseContext.Database.MigrateAsync();