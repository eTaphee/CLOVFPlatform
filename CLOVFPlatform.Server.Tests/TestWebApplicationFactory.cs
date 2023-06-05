using CLOVFPlatform.Server.Models;
using CLOVFPlatform.Server.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CLOVFPlatform.Server.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0

            builder.ConfigureServices(services =>
            {
                builder.ConfigureLogging(logging => { logging.ClearProviders(); });

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(CLOVFContext));

                services.Remove(descriptor!);

                services.AddDbContext<CLOVFContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<CLOVFContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<TestWebApplicationFactory>>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    try
                    {
                        Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}
