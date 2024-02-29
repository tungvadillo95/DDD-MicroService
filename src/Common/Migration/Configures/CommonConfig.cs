using Core.Common;
using Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Migration.Configures
{
    public class CommonConfig
    {
        public static IConfiguration GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices((hostContext, services) =>
            {
                //Add DB context
                services.AddDbContext<ExampleDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString(AppConstants.MainConnectionString));
                });
            });
            return builder;
        }
    }
}
