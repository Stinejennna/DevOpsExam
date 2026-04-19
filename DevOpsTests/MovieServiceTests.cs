namespace DevOpsTests;

using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;

public class MovieServiceTests
{
    [Fact]
    public void AddMovie_ValidMovie_AddsSuccessfully()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "Test", Rating = 5 });

        Assert.Single(service.GetAll());
    }
    
    [Fact]
    public void AddMovie_RatingTooLow_ThrowsException()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentException>(() =>
            service.AddMovie(new Movie { Title = "Bad", Rating = 0 }));
    }
    
    [Fact]
    public void AddMovie_RatingTooHigh_ThrowsException()
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
    public void AddMovie_Null_ThrowsException()
    {
        var service = new MovieService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddMovie(null));
    }
    
    [Fact]
    public void GetAverageRating_ReturnsCorrectAverage()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });
        service.AddMovie(new Movie { Title = "B", Rating = 7 });

        var avg = service.GetAverageRating();

        Assert.Equal(6, avg, 1);
    }
    
    [Fact]
    public void GetAverageRating_NoMovies_ReturnsZero()
    {
        var service = new MovieService();

        var avg = service.GetAverageRating();

        Assert.Equal(0, avg);
    }
    
    [Fact]
    public void GetAll_ReturnsAllMovies()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });
        service.AddMovie(new Movie { Title = "B", Rating = 7 });

        var movies = service.GetAll();

        Assert.Equal(2, movies.Count);
    }
    
    [Fact]
    public void GetAverageRating_OneMovie_ReturnsThatRating()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 8 });

        Assert.Equal(8, service.GetAverageRating());
    }
    
    [Fact]
    public void GetAverageRating_SameRatings_ReturnsSame()
    {
        var service = new MovieService();

        service.AddMovie(new Movie { Title = "A", Rating = 5 });
        service.AddMovie(new Movie { Title = "B", Rating = 5 });

        Assert.Equal(5, service.GetAverageRating());
    }
}