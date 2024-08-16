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
        services.AddOpenTelemetryTracing(Configuration);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // configure AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

        var telemetryApiUrl = Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
        var deployingEnvironment = Configuration.GetValue<string>("DeployingEnvironment");
        services.ConfigureTelemetryClientWithLogger(
            "AntiClownBot" + (string.IsNullOrEmpty(deployingEnvironment) ? "" : $"_{deployingEnvironment}"),
            "DataApi",
            telemetryApiUrl
        );

        // configure database
        services.ConfigureConnectionStringFromAppSettings(Configuration.GetSection("PostgreSql"))
                .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
                .ConfigurePostgreSql();

        // configure repositories
        services.AddTransientWithProxy<ISettingsRepository, SettingsRepository>();
        services.AddTransientWithProxy<ITokensRepository, TokensRepository>();
        services.AddTransientWithProxy<IRightsRepository, RightsRepository>();

        // configure services
        services.AddTransientWithProxy<ISettingsService, SettingsService>();
        services.AddTransientWithProxy<ITokenGenerator, GuidTokenGenerator>();
        services.AddTransientWithProxy<ITokensService, TokensService>();
        services.AddTransientWithProxy<IRightsService, RightsService>();

        services.AddControllers().AddNewtonsoftJson(
            options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
            }
        );
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