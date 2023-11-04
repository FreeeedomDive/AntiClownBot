using AntiClown.Data.Api.Core.Database;
using AntiClown.Data.Api.Core.Options;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.SettingsStoring.Services;
using AntiClown.Data.Api.Core.Tokens.Repositories;
using AntiClown.Data.Api.Core.Tokens.Services;
using AntiClown.Data.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SqlRepositoryBase.Configuration.Extensions;
using TelemetryApp.Utilities.Extensions;
using TelemetryApp.Utilities.Middlewares;

namespace AntiClown.Data.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // configure AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

        services.Configure<DatabaseOptions>(Configuration.GetSection("PostgreSql"));
        var telemetryApiUrl = Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
        services.ConfigureTelemetryClientWithLogger("AntiClownBot", "DataApi", telemetryApiUrl);

        // configure database
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.ConfigurePostgreSql();

        // configure repositories
        services.AddTransient<ISettingsRepository, SettingsRepository>();
        services.AddTransient<ITokensRepository, TokensRepository>();

        // configure services
        services.AddTransient<ISettingsService, SettingsService>();
        services.AddTransient<ITokenGenerator, GuidTokenGenerator>();
        services.AddTransient<ITokensService, TokensService>();

        services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.TypeNameHandling = TypeNameHandling.All);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public IConfiguration Configuration { get; }
}