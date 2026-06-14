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

    [Fact]
    public void Application_StartsAndRegistersServices_Successfully()
    {
        var client = _factory.CreateClient();
        
        Assert.NotNull(client);
    }
    
    [Fact]
    public void Application_Starts_InDevelopment()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        var client = factory.CreateClient();

        Assert.NotNull(client);
    }
    
    [Fact]
    public async Task Application_Starts_InDevelopment_WithDatabase()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("testdb"));
                });
            });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.NotNull(response);
    }
    
    [Fact]
    public void Program_Constructor_CanBeCreated()
    {
        var program = new Program();

        Assert.NotNull(program);
    }
}