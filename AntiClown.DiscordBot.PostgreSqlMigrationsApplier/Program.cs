using AntiClown.DiscordBot.Database;
using AntiClown.DiscordBot.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var postgresSection = builder.Configuration.GetSection("PostgreSql");
builder.Services.Configure<DatabaseOptions>(postgresSection);
builder.Services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);

var app = builder.Build();

var databaseContext = app.Services.GetService<DatabaseContext>()!;
await databaseContext.Database.MigrateAsync();