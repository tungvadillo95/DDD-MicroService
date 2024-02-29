using Infrastructure.EF;
using Microsoft.AspNetCore.Identity;
using Migration.Configures;
using Migration.Extensions;

var configuration = CommonConfig.GetConfiguration();
var host = CommonConfig.CreateHostBuilder(configuration, args).Build();
host.MigrateDbContext<ExampleDbContext>((context, services) =>
{
    var userManager = services.GetService(typeof(UserManager<IdentityUser>)) as UserManager<IdentityUser>;
    ExampleDbContextSeed.SeedAsync(context).Wait();

    host.Run();
});