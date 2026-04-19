using DevOpsExamMovie.Data;
using DevOpsExamMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsExamMovie.Services;

public class MovieService
{
    private readonly AppDbContext _context;

    public MovieService(AppDbContext context)
    {
        _context = context;
    }

    public void AddMovie(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        if (string.IsNullOrWhiteSpace(movie.Title))
            throw new ArgumentException("Title is required");

        if (movie.Rating is null || movie.Rating < 1 || movie.Rating > 10)
            throw new ArgumentException("Rating must be between 1 and 10");

        _context.Movies.Add(movie);
        _context.SaveChanges();
    }

    public double GetAverageRating()
    {
        if (!_context.Movies.Any())
            return 0;

        return _context.Movies.Average(m => m.Rating!.Value);
    }

    public List<Movie> GetAll()
    {
        return _context.Movies.ToList();
    }
}