using AntiClownApi.AdminApi.SettingsServices.App;
using AntiClownApi.AdminApi.SettingsServices.Events;
using AntiClownApi.AdminApi.SettingsServices.Guild;

namespace AntiClownApi.AdminApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddTransient<IAppSettingsService, AppSettingsService>();
        services.AddTransient<IEventSettingsService, EventSettingsService>();
        services.AddTransient<IGuildSettingsService, GuildSettingsService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AntiClownApi v1"));
        }
        
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public IConfiguration Configuration { get; }
}