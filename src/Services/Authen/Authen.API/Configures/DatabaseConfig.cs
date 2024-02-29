using Core.Common;
using Infrastructure.EF;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Authen.API.Configures
{
    public class DatabaseConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            // SQL - Microsoft.EntityFrameworkCore.SqlServer
            services.AddDbContext<ExampleDbContext>(options =>
            {
                options.UseLoggerFactory(ConsoleLoggerFactory);

                options.UseNpgsql(configuration.GetConnectionString(AppConstants.MainConnectionString),
                   oracleOption =>
                   {
                       oracleOption.MigrationsAssembly(typeof(ExampleDbContext).GetTypeInfo().Assembly.GetName().Name);
                   });

                options.EnableSensitiveDataLogging(true);
            });

            // Inject UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder
            .AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Debug)
            .AddConsole();
        });
    }
}
