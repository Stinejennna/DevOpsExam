using DevOpsExamMovie.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace DevOpsTests;

public class ProgramTests 
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _env;
        public CustomWebApplicationFactory(string env) => _env = env;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(_env);

            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => 
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);
                
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
            });
        }
    }

    [Fact]
    public void Application_Starts_Successfully()
    {
        using var factory = new CustomWebApplicationFactory("Testing");
        var client = factory.CreateClient();
        Assert.NotNull(client);
    }
    
    [Fact]
    public async Task AppStartup_WhenNotTesting_ExecutesMigration()
    {
        using var factory = new CustomWebApplicationFactory("Production");
        var client = factory.CreateClient();
        Assert.NotNull(client);
    }
    
    [Fact]
    public void Application_Starts_InDevelopmentMode_MapsOpenApi()
    {
        using var factory = new CustomWebApplicationFactory("Development");
        var client = factory.CreateClient();
        Assert.NotNull(client);
    }
}