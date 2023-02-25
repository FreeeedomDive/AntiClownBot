using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using AutoMapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Configuration.Extensions;

namespace AntiClown.Api;

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

        // configure database
        var postgresSection = Configuration.GetSection("PostgreSql");
        services.Configure<DatabaseOptions>(postgresSection);
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.ConfigurePostgreSql();

        // configure repositories
        services.AddTransient<IUsersRepository, UsersRepository>();
        
        // configure services
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<INewUserService, NewUserService>();
        
        // configure HangFire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(postgresSection["ConnectionString"])
        );

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseHangfireServer();
        app.UseHangfireDashboard();
    }

    public IConfiguration Configuration { get; }
}