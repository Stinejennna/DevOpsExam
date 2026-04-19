using DevOpsExamMovie.Controllers;
using DevOpsExamMovie.Data;
using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsTests;

public class MovieServiceTests
{
    private MovieService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        return new MovieService(context);
    }

    [Fact]
    public void AddMovie_Valid_AddsMovie()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "Inception", Rating = 8 });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public void AddMovie_Null_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentNullException>(() => service.AddMovie(null!));
    }

    [Fact]
    public void AddMovie_InvalidTitle_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "", Rating = 5 }));
    }

    [Fact]
    public void AddMovie_NullRating_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "A", Rating = null }));
    }

    [Fact]
    public void AddMovie_RatingTooLow_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "A", Rating = 0 }));
    }

    [Fact]
    public void AddMovie_RatingTooHigh_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "A", Rating = 11 }));
    }

    [Fact]
    public void AddMovie_Boundaries_Work()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "Min", Rating = 1 });
        service.AddMovie(new Movie { Title = "Max", Rating = 10 });

        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public void GetAverage_NoMovies_ReturnsZero()
    {
        var service = CreateService();

        Assert.Equal(0, service.GetAverageRating());
    }

    [Fact]
    public void GetAverage_OneMovie_ReturnsValue()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "A", Rating = 10 });

        Assert.Equal(10, service.GetAverageRating());
    }

    [Fact]
    public void GetAverage_MultipleMovies_ReturnsCorrect()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "A", Rating = 6 });
        service.AddMovie(new Movie { Title = "B", Rating = 8 });

        Assert.Equal(7, service.GetAverageRating());
    }

    [Fact]
    public void GetAll_ReturnsMovies()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });
        service.AddMovie(new Movie { Title = "B", Rating = 6 });

        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public void GetAll_ReturnsCopy()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });

        var list = service.GetAll();
        list.Clear();

        Assert.Single(service.GetAll());
    }

    [Fact]
    public void Controller_Add_ReturnsOk()
    {
        var service = CreateService();
        var controller = new MoviesController(service);

        var result = controller.Add(new Movie { Title = "A", Rating = 5 });

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void Controller_GetAll_ReturnsOkObject()
    {
        var service = CreateService();
        service.AddMovie(new Movie { Title = "A", Rating = 5 });

        var controller = new MoviesController(service);

        var result = controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Controller_GetAverage_ReturnsOkObject()
    {
        var service = CreateService();
        service.AddMovie(new Movie { Title = "A", Rating = 10 });

        var controller = new MoviesController(service);

        var result = controller.GetAverage();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(10d, (double)ok.Value!);
    }
    
    [Fact]
    public void Controller_Add_NullMovie_ReturnsBadRequest()
    {
        var controller = new MoviesController(CreateService());

        var result = controller.Add(null!);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Controller_GetAll_ReturnsMovies()
    {
        var service = CreateService();
        service.AddMovie(new Movie { Title = "A", Rating = 5 });

        var controller = new MoviesController(service);

        var result = controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var movies = Assert.IsAssignableFrom<IEnumerable<Movie>>(ok.Value);

        Assert.Single(movies);
    }

    [Fact]
    public void AddMovie_TitleWhitespace_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "   ", Rating = 5 }));
    }

    [Fact]
    public void GetAverage_WithThreeMovies_ReturnsCorrect()
    {
        var service = CreateService();

        service.AddMovie(new Movie { Title = "A", Rating = 2 });
        service.AddMovie(new Movie { Title = "B", Rating = 4 });
        service.AddMovie(new Movie { Title = "C", Rating = 6 });

        Assert.Equal(4, service.GetAverageRating());
    }
}