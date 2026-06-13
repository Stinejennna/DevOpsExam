using DevOpsExamMovie.Controllers;
using DevOpsExamMovie.Data;
using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevOpsTests;

public class MovieServiceTests
{
    private static MovieService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var settings = new Dictionary<string, string?>
        {
            { "TMDB:ApiKey", "test-key" }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new MovieService(context, config);
    }
    
    private static MovieService CreateServiceWithKey(string key)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var settings = new Dictionary<string, string?>
        {
            { "TMDB:ApiKey", key }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new MovieService(context, config);
    }

    [Fact]
    public async Task AddMovie_Valid_AddsMovie()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 8
        });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public async Task AddMovie_InvalidTitle_Throws()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddMovie(new Movie
            {
                Title = "",
                Rating = 5
            }));
    }

    [Fact]
    public async Task AddMovie_InvalidRating_Throws()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddMovie(new Movie
            {
                Title = "Batman",
                Rating = 11
            }));
    }

    [Fact]
    public async Task GetAverage_ReturnsCorrect()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title = "A", Rating = 6 });
        await service.AddMovie(new Movie { Title = "B", Rating = 8 });

        Assert.Equal(7, service.GetAverageRating());
    }

    [Fact]
    public async Task DeleteMovie_RemovesMovie()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 8
        });

        var movie = service.GetAll().First();

        service.DeleteMovie(movie.Id);

        Assert.Empty(service.GetAll());
    }

    [Fact]
    public async Task UpdateRating_ChangesRating()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 5
        });

        var movie = service.GetAll().First();

        service.UpdateRating(movie.Id, 9);

        Assert.Equal(9, service.GetAll().First().Rating);
    }

    [Fact]
    public async Task Controller_Add_ReturnsOk()
    {
        var service = CreateService();
        var controller = new MoviesController(service);

        var result = await controller.Add(new CreateMovieRequest
        {
            Title = "Batman",
            Rating = 8
        });

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Controller_GetAll_ReturnsOkObject()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 8
        });

        var controller = new MoviesController(service);

        var result = controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Controller_Delete_ReturnsOk()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 8
        });

        var movie = service.GetAll().First();

        var controller = new MoviesController(service);

        var result = controller.Delete(movie.Id);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Controller_UpdateRating_ReturnsOk()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 5
        });

        var movie = service.GetAll().First();

        var controller = new MoviesController(service);

        var result = controller.UpdateRating(
            movie.Id,
            new RatingUpdate { Rating = 10 });

        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public void DeleteMovie_InvalidId_DoesNothing()
    {
        var service = CreateService();

        service.DeleteMovie(999);

        Assert.Empty(service.GetAll());
    }

    [Fact]
    public async Task UpdateRating_ChangesAverage()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title="A", Rating=5 });

        var movie = service.GetAll().First();

        service.UpdateRating(movie.Id, 10);

        Assert.Equal(10, service.GetAverageRating());
    }

    [Fact]
    public void GetAverage_NoMovies_ReturnsZero()
    {
        var service = CreateService();

        Assert.Equal(0, service.GetAverageRating());
    }

    [Fact]
    public void UpdateRating_InvalidId_DoesNothing()
    {
        var service = CreateService();

        service.UpdateRating(999, 5);

        Assert.Empty(service.GetAll());
    }
    
    [Fact]
    public void Controller_GetAverage_ReturnsOk()
    {
        var service = CreateService();
        var controller = new MoviesController(service);

        var result = controller.GetAverage();

        Assert.IsType<OkObjectResult>(result);
    }
    
     [Fact]
public async Task AddMovie_TestKey_AddsMetadata()
{
    var service = CreateService();

    await service.AddMovie(new Movie
    {
        Title = "Batman",
        Rating = 8
    });

    var movie = service.GetAll().First();

    Assert.Equal("2020", movie.ReleaseYear);
    Assert.Equal("Test", movie.Genre);
    Assert.Equal("", movie.PosterUrl);
    }

    [Fact]
    public async Task AddMovie_InvalidLowRating_Throws()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddMovie(new Movie
            {
                Title = "Batman",
                Rating = 0
            }));
    }

    [Fact]
    public async Task AddMovie_InvalidHighRating_Throws()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddMovie(new Movie
            {
                Title = "Batman",
                Rating = 11
            }));
    }

    [Fact]
    public async Task UpdateRating_UpdatesAverage()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title = "A", Rating = 5 });
        await service.AddMovie(new Movie { Title = "B", Rating = 5 });

        var movie = service.GetAll().First();

        service.UpdateRating(movie.Id, 10);

        Assert.Equal(7.5, service.GetAverageRating());
    }


    [Fact]
    public async Task Controller_Add_Null_ReturnsBadRequest()
    {
        var service = CreateService();
        var controller = new MoviesController(service);

        var result = await controller.Add(null!);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Controller_GetAverage_ReturnsOkObject()
    {
        var service = CreateService();
        var controller = new MoviesController(service);

        var result = controller.GetAverage();

        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task AddMovie_RatingOne_IsValid()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Min",
            Rating = 1
        });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public async Task AddMovie_RatingTen_IsValid()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Max",
            Rating = 10
        });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public async Task GetAll_ReturnsAddedMovies()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title = "A", Rating = 5 });
        await service.AddMovie(new Movie { Title = "B", Rating = 6 });

        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public async Task DeleteMovie_OnlyRemovesCorrectMovie()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title = "A", Rating = 5 });
        await service.AddMovie(new Movie { Title = "B", Rating = 6 });

        var first = service.GetAll().First();

        service.DeleteMovie(first.Id);

        Assert.Single(service.GetAll());
        Assert.Equal("B", service.GetAll().First().Title);
    }

    [Fact]
    public async Task UpdateRating_CanUpdateTwice()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 5
        });

        var movie = service.GetAll().First();

        service.UpdateRating(movie.Id, 8);
        service.UpdateRating(movie.Id, 10);

        Assert.Equal(10, service.GetAll().First().Rating);
    }

    [Fact]
    public async Task GetAverage_WithOneMovie_ReturnsExactValue()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Solo",
            Rating = 9
        });

        Assert.Equal(9, service.GetAverageRating());
    }

    [Fact]
    public async Task Controller_Delete_InvalidId_ReturnsOk()
    {
        var service = CreateService();
        var controller = new MoviesController(service);

        var result = controller.Delete(999);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Controller_UpdateRating_ChangesMovieRating()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 5
        });

        var movie = service.GetAll().First();

        var controller = new MoviesController(service);

        controller.UpdateRating(movie.Id,
            new RatingUpdate { Rating = 9 });

        Assert.Equal(9, service.GetAll().First().Rating);
    }

    [Fact]
    public async Task Metadata_TestKey_SetsReleaseYear()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Movie",
            Rating = 7
        });

        Assert.Equal("2020",
            service.GetAll().First().ReleaseYear);
    }

    [Fact]
    public async Task Metadata_TestKey_SetsGenre()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Movie",
            Rating = 7
        });

        Assert.Equal("Test",
            service.GetAll().First().Genre);
    }
    
    [Fact]
    public async Task UpdateRating_InvalidLow_Throws()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 5
        });

        var movie = service.GetAll().First();

        Assert.Throws<ArgumentException>(() =>
            service.UpdateRating(movie.Id, 0));
    }
    
    [Fact]
    public async Task UpdateRating_InvalidHigh_Throws()
    {
        var service = CreateService();

        await service.AddMovie(new Movie
        {
            Title = "Batman",
            Rating = 5
        });

        var movie = service.GetAll().First();

        Assert.Throws<ArgumentException>(() =>
            service.UpdateRating(movie.Id, 11));
    }

    [Fact]
    public async Task DeleteMovie_AfterDelete_AverageBecomesZero()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title="A", Rating=8 });

        var movie = service.GetAll().First();

        service.DeleteMovie(movie.Id);

        Assert.Equal(0, service.GetAverageRating());
    }

    [Fact]
    public async Task GetAll_ReturnsCopy()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title="A", Rating=5 });

        var list = service.GetAll();

        list.Clear();

        Assert.Single(service.GetAll());
    }

    [Fact]
    public async Task Controller_UpdateRating_ReturnsOkResult()
    {
        var service = CreateService();

        await service.AddMovie(new Movie { Title="A", Rating=5 });

        var movie = service.GetAll().First();

        var controller = new MoviesController(service);

        var result = controller.UpdateRating(movie.Id,
            new RatingUpdate { Rating = 8 });

        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task AddMovie_Null_Throws()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.AddMovie(null!));
    }
    
    [Fact]
    public async Task UpdateRating_InvalidId_DoesNothing_NoCrash()
    {
        var service = CreateService();

        service.UpdateRating(999, 5);

        Assert.Empty(service.GetAll());
    }
    
    [Fact]
    public void DeleteMovie_InvalidId_NoException()
    {
        var service = CreateService();

        service.DeleteMovie(999);

        Assert.Empty(service.GetAll());
    }
    
    [Theory]
    [InlineData(28, "Action")]
    [InlineData(12, "Adventure")]
    [InlineData(16, "Animation")]
    [InlineData(35, "Comedy")]
    [InlineData(80, "Crime")]
    public void GenreMapping_Works(int id, string expected)
    {
        var method = typeof(MovieService)
            .GetMethod("GetGenreName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = method!.Invoke(null, new object[] { id });

        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void DeleteMovie_InvalidId_NoChanges()
    {
        var service = CreateService();

        service.DeleteMovie(999);

        Assert.Empty(service.GetAll());
    }
    
    [Fact]
    public void UpdateRating_InvalidId_NoChanges()
    {
        var service = CreateService();

        service.UpdateRating(999, 5);

        Assert.Empty(service.GetAll());
    }
    
    [Fact]
    public async Task AddMovie_NoResultsKey_SetsDefaultValues()
    {
        var service = CreateServiceWithKey("noresults-test");
        
        await service.AddMovie(new Movie { Title = "UnknownMovie", Rating = 5 });
        
        var movie = service.GetAll().First();
        Assert.Equal("https://via.placeholder.com/300x450?text=No+Poster", movie.PosterUrl);
        Assert.Equal("-", movie.ReleaseYear);
        Assert.Equal("Unknown", movie.Genre);
    }

    [Fact]
    public async Task AddMovie_NoPosterKey_SetsDefaultPoster()
    {
        var service = CreateServiceWithKey("noposter-test");
        
        await service.AddMovie(new Movie { Title = "NoPosterMovie", Rating = 5 });
        
        var movie = service.GetAll().First();
        Assert.Equal("https://via.placeholder.com/300x450?text=No+Poster", movie.PosterUrl);
        Assert.Equal("2020", movie.ReleaseYear);
        Assert.Equal("Test", movie.Genre);
    }
}