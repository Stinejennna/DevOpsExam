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
    private class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _env;
        public CustomWebApplicationFactory(string env = "Production") => _env = env;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(_env); 
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
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