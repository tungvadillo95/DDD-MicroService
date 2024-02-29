using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class Program
{
    private static readonly string Namespace = typeof(Program).Namespace;
    public static readonly string AppName = Namespace.Split('.')[0];

    public static int Main(string[] args)
    {
        var configuration = Helpers.GetConfiguration();

        Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));
        try
        {
            Log.Information("Configuring web host ({MSDbContext})...", AppName);
            var host = CreateHostBuilder(configuration, args).Build();
            Log.Information("Applying migrations ({MSDbContext})...", AppName);
            host.MigrateDbContext<MSDbContext>((context, services) =>
            {
                var userManager = (UserManager<IdentityUser>)services.GetService(typeof(UserManager<IdentityUser>));
                MSDbContextSeed.SeedAsync(context).Wait();
            });

            Log.Information("Starting web host ({MSDbContext})...", AppName);
            host.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Program terminated unexpectedly ({MSDbContext})!", AppName);
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        builder.UseSerilog();
        builder.ConfigureServices((hostContext, services) =>
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<MSDbContext>()
            .AddDefaultTokenProviders();

            services.AddSingleton(new AppModule { Name = "Migration" });
            //Add DB context
            services.AddDbContext<MSDbContext>(options =>
            {
                options.UseOracle(configuration.GetConnectionString(AppConstants.MainConnectionString));
            });
        });
        return builder;
    }

}
