namespace DevOpsTests;

using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;
public class MovieServiceTests
{
    [Fact]
    public void AddMovie_ValidMovie_AddsMovie()
    {
        var service = new MovieService();

        service.AddMovie(new Movie
        {
            Title = "Inception",
            Rating = 8
        });

        Assert.Single(service.GetAll());
    }

    [Fact]
    public void AddMovie_NullMovie_ThrowsException()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddMovie(null!));
    }

    [Fact]
    public void AddMovie_InvalidLowRating_ThrowsException()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "Bad", Rating = 0 }));
    }

    [Fact]
    public void AddMovie_InvalidHighRating_ThrowsException()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "Bad", Rating = 11 }));
    }

    [Fact]
    public void AddMovie_EmptyTitle_ThrowsException()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "", Rating = 5 }));
    }

    [Fact]
    public void GetAverageRating_NoMovies_ReturnsZero()
    {
        var service = new MovieService();

        var result = service.GetAverageRating();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetAverageRating_ReturnsCorrectAverage()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 6 });
        service.AddMovie(new Movie { Title = "B", Rating = 8 });

        var result = service.GetAverageRating();

        Assert.Equal(7, result);
    }

    [Fact]
    public void GetAll_ReturnsAllMovies()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });
        service.AddMovie(new Movie { Title = "B", Rating = 7 });

        var result = service.GetAll();

        Assert.Equal(2, result.Count);
    }
}