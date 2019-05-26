using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeApi.IntegrationTests
{
    ///  <summary>
    /// Factory for bootstrapping Employee API in memory for functional end to end tests.
    /// 
    ///  </summary>
    public class EmployeeControllerTestFactory : WebApplicationFactory<Startup>
    {
        /// <summary>
        ///  Web host configuration for functional tests
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.UseUrls("http://localhost:5000");
            builder.ConfigureAppConfiguration((builderContext, config) => { config.AddJsonFile("appsettings.json"); });
            builder.ConfigureServices((context, services) =>
            {
                var connection = new SqliteConnection(context.Configuration["connString"]);
                connection.Open();
                services.AddDbContext<EmployeeDbContext>(options => options.UseSqlite(connection));
                //Build the service provider
                var sp = services.BuildServiceProvider();

                //create a scope to obtain a reference to database context
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<EmployeeDbContext>();
                    try
                    {
                        // Ensure the database is created.
                        db.Database.EnsureCreated();
                        //inject seed data - precondition for your integration tests to run
                        db.Employees.Add(new Employee()
                        {
                            Id = 1,
                            Name = "John",
                            City = "Los Angeles"
                        });
                        db.Employees.Add(new Employee()
                        {
                            Id = 2,
                            Name = "Mike",
                            City = "Chicago"
                        });
                        db.Employees.Add(new Employee()
                        {
                            Id = 3,
                            Name = "David",
                            City = "SunnyVale"
                        });
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred creating database. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}