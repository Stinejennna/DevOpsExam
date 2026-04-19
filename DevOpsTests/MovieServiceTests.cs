using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;
using DevOpsExamMovie.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsTests;

public class MovieServiceTests
{

    [Fact]
    public void AddMovie_Valid_AddsMovie()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "Inception", Rating = 8 });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public void AddMovie_Null_Throws()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddMovie(null!));
    }

    [Fact]
    public void AddMovie_InvalidTitle_Throws()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "", Rating = 5 }));
    }

    [Fact]
    public void AddMovie_RatingOutOfRange_Throws()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "Bad", Rating = 0 }));

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "Bad", Rating = 11 }));
    }
    
    [Fact]
    public void AddMovie_BelowLowerBoundary_Fails()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "A", Rating = 0 }));
    }

    [Fact]
    public void AddMovie_AboveUpperBoundary_Fails()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "A", Rating = 11 }));
    }

    [Fact]
    public void AddMovie_ValidBoundaries_Work()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 1 });
        service.AddMovie(new Movie { Title = "B", Rating = 10 });

        Assert.Equal(2, service.GetAll().Count);
    }
    
    [Fact]
    public void AddMovie_InvalidTitle_MessageIsCorrect()
    {
        var service = new MovieService();

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "", Rating = 5 }));

        Assert.Equal("Title is required", ex.Message);
    }

    [Fact]
    public void AddMovie_InvalidRating_MessageIsCorrect()
    {
        var service = new MovieService();

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "A", Rating = 0 }));

        Assert.Equal("Rating must be between 1 and 10", ex.Message);
    }

    [Fact]
    public void GetAverage_NoMovies_ReturnsZero()
    {
        var service = new MovieService();

        Assert.Equal(0, service.GetAverageRating());
    }

    [Fact]
    public void GetAverage_OneMovie_ReturnsValue()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 10 });

        Assert.Equal(10, service.GetAverageRating());
    }

    [Fact]
    public void GetAverage_MultipleMovies_ReturnsCorrectValue()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 6 });
        service.AddMovie(new Movie { Title = "B", Rating = 8 });

        Assert.Equal(7, service.GetAverageRating());
    }

    [Fact]
    public void GetAll_ReturnsSameItems()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });
        service.AddMovie(new Movie { Title = "B", Rating = 6 });

        var result = service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_ReturnsCopy_NotInternalList()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });

        var list = service.GetAll();
        list.Clear();

        Assert.Single(service.GetAll()); // ensures ToList() is tested
    }

    [Fact]
    public void Controller_Add_ReturnsOk()
    {
        var service = new MovieService();
        var controller = new MoviesController(service);

        var result = controller.Add(new Movie { Title = "Test", Rating = 5 });

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void Controller_Add_Valid_CallsService()
    {
        var service = new MovieService();
        var controller = new MoviesController(service);

        controller.Add(new Movie { Title = "A", Rating = 5 });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public void Controller_GetAll_ReturnsOkWithData()
    {
        var service = new MovieService();
        service.AddMovie(new Movie { Title = "A", Rating = 5 });

        var controller = new MoviesController(service);

        var result = controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single((IEnumerable<Movie>)ok.Value!);
    }

    [Fact]
    public void Controller_GetAverage_ReturnsOk()
    {
        var service = new MovieService();
        service.AddMovie(new Movie { Title = "A", Rating = 10 });

        var controller = new MoviesController(service);

        var result = controller.GetAverage();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(10, ok.Value);
    }
}