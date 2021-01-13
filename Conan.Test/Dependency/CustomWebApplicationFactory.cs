using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Conan.Domain;
using Conan.Infrastructure;
using Conan.UnitTest.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conan.Test.Dependency
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(IRepository<>));
                services.Remove(descriptor);

                services.AddScoped(typeof(IRepository<>), typeof(InMemoryRepository<>));
                //services.AddScoped<ITransactionControl, FakeTransactionControl>();

                var sp = services.BuildServiceProvider();

                //using (var scope = sp.CreateScope())
                //{
                //    var scopedServices = scope.ServiceProvider;
                //    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                //    var logger = scopedServices
                //        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                //    db.Database.EnsureCreated();

                //    try
                //    {
                //        Utilities.InitializeDbForTests(db);
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.LogError(ex, "An error occurred seeding the " +
                //            "database with test messages. Error: {Message}", ex.Message);
                //    }
                //}
            });
        }
    }
}
