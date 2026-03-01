using AntiClown.Core.OpenTelemetry;
using AntiClown.Data.Api.Core.Database;
using AntiClown.Data.Api.Core.Rights.Repositories;
using AntiClown.Data.Api.Core.Rights.Services;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.SettingsStoring.Services;
using AntiClown.Data.Api.Core.Tokens.Repositories;
using AntiClown.Data.Api.Core.Tokens.Services;
using AntiClown.Data.Api.Middlewares;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using SqlRepositoryBase.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddOpenTelemetryTracing(builder.Configuration);

// configure AutoMapper
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

// configure database
builder.Services.ConfigurePostgreSql<DatabaseContext>(builder.Configuration.GetSection("PostgreSql"));

// configure repositories
builder.Services.AddTransientWithProxy<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransientWithProxy<ITokensRepository, TokensRepository>();
builder.Services.AddTransientWithProxy<IRightsRepository, RightsRepository>();

// configure services
builder.Services.AddTransientWithProxy<ISettingsService, SettingsService>();
builder.Services.AddTransientWithProxy<ITokenGenerator, GuidTokenGenerator>();
builder.Services.AddTransientWithProxy<ITokensService, TokensService>();
builder.Services.AddTransientWithProxy<IRightsService, RightsService>();

builder.Services.AddControllers().AddNewtonsoftJson(
    options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
    }
);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();
app.UseWebSockets();

app.UseSerilogRequestLogging();
app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();

public partial class Program { }
