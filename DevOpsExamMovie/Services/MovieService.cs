namespace DevOpsExamMovie.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Models;

public class MovieService
{
    private readonly List<Movie> _movies = new();

    public void AddMovie(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        if (string.IsNullOrWhiteSpace(movie.Title))
            throw new ArgumentException("Title is required");

        if (movie.Rating < 1 || movie.Rating > 10)
            throw new ArgumentException("Rating must be between 1 and 10");

        _movies.Add(movie);
    }

    public double GetAverageRating()
    {
        if (_movies.Count == 0)
            return 0;

        return _movies.Average(m => m.Rating);
    }

    public List<Movie> GetAll()
    {
        return _movies.ToList();
    }
}