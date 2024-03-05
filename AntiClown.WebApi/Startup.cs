using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.DiscordApi.Client;
using AntiClown.DiscordApi.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.WebApi.Middlewares;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using TelemetryApp.Utilities.Extensions;
using TelemetryApp.Utilities.Middlewares;

namespace AntiClown.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var telemetryApiUrl = Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
        var deployingEnvironment = Configuration.GetValue<string>("DeployingEnvironment");
        services.ConfigureTelemetryClientWithLogger(
            "AntiClownBot" + (string.IsNullOrEmpty(deployingEnvironment) ? "" : $"_{deployingEnvironment}"),
            "WebApi",
            telemetryApiUrl
        );

        services.Configure<AntiClownDataApiConnectionOptions>(Configuration.GetSection("AntiClownDataApi"));
        services.Configure<AntiClownApiConnectionOptions>(Configuration.GetSection("AntiClownApi"));
        services.Configure<AntiClownEntertainmentApiConnectionOptions>(Configuration.GetSection("AntiClownEntertainmentApi"));
        services.Configure<AntiClownDiscordApiConnectionOptions>(Configuration.GetSection("AntiClownDiscordApi"));

        services.AddTransient<IAntiClownDataApiClient>(
            serviceProvider => AntiClownDataApiClientProvider.Build(
                serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl
            )
        );
        services.AddTransient<IAntiClownApiClient>(
            serviceProvider => AntiClownApiClientProvider.Build(
                serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl
            )
        );
        services.AddTransient<IAntiClownEntertainmentApiClient>(
            serviceProvider => AntiClownEntertainmentApiClientProvider.Build(
                serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl
            )
        );
        services.AddTransient<IAntiClownDiscordApiClient>(
            serviceProvider => AntiClownDiscordApiClientProvider.Build(
                serviceProvider.GetRequiredService<IOptions<AntiClownDiscordApiConnectionOptions>>().Value.ServiceUrl
            )
        );

        services.AddControllers().AddNewtonsoftJson(
            options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
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