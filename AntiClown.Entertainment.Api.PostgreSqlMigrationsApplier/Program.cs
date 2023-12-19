using AntiClown.Entertainment.Api.Core.Database;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.ContextBuilders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureConnectionStringFromAppSettings(builder.Configuration.GetSection("PostgreSql"))
       .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
       .ConfigurePostgreSql();

var app = builder.Build();

var databaseContextFactory = app.Services.GetRequiredService<IDbContextFactory>();
var databaseContext = databaseContextFactory.Build();
await databaseContext.Database.MigrateAsync();