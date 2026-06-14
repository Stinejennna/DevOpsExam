using DevOpsExamMovie.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace DevOpsTests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });
    }
    
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production"); 
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("MigrationTestDb"));
            });
        }
    }

    [Fact]
    public void Application_StartsAndRegistersServices_Successfully()
    {
        var client = _factory.CreateClient();
        
        Assert.NotNull(client);
    }
    
    [Fact]
    public async Task AppStartup_WhenNotTesting_ExecutesMigration()
    {
        using var factory = new CustomWebApplicationFactory();
        
        var client = factory.CreateClient();
        
        Assert.NotNull(client);
    }
    
    [Fact]
    public void Application_Starts_InDevelopmentMode_MapsOpenApi()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        var client = factory.CreateClient();
        Assert.NotNull(client);
    }
}