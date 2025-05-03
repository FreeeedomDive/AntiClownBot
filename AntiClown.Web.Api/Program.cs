using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Core.OpenTelemetry;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.DiscordBot.Client;
using AntiClown.DiscordBot.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.Web.Api.ExternalClients.F1FastApi;
using AntiClown.Web.Api.Middlewares;
using AntiClown.Web.Api.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddOpenTelemetryTracing(builder.Configuration);

builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));
builder.Services.Configure<AntiClownApiConnectionOptions>(builder.Configuration.GetSection("AntiClownApi"));
builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));
builder.Services.Configure<AntiClownDiscordApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDiscordApi"));

builder.Services.Configure<F1FastApiOptions>(builder.Configuration.GetSection("F1FastApi"));

builder.Services.AddTransientWithProxy<IAntiClownDataApiClient>(
    serviceProvider => AntiClownDataApiClientProvider.Build(
        serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl
    )
);
builder.Services.AddTransientWithProxy<IAntiClownApiClient>(
    serviceProvider => AntiClownApiClientProvider.Build(
        serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl
    )
);
builder.Services.AddTransientWithProxy<IAntiClownEntertainmentApiClient>(
    serviceProvider => AntiClownEntertainmentApiClientProvider.Build(
        serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl
    )
);
builder.Services.AddTransientWithProxy<IAntiClownDiscordBotClient>(
    serviceProvider => AntiClownDiscordApiClientProvider.Build(
        serviceProvider.GetRequiredService<IOptions<AntiClownDiscordApiConnectionOptions>>().Value.ServiceUrl
    )
);

builder.Services.AddTransient<IF1FastApiClient, F1FastApiClient>();

builder.Services.AddControllers().AddNewtonsoftJson(
    options => options.SerializerSettings.Converters.Add(new StringEnumConverter())
);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();
app.UseWebSockets();

app.UseSerilogRequestLogging();
app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();