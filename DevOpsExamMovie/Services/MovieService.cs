using DevOpsExamMovie.Data;
using DevOpsExamMovie.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DevOpsExamMovie.Services;

public class MovieService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public MovieService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task AddMovie(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        if (string.IsNullOrWhiteSpace(movie.Title))
            throw new ArgumentException("Title is required");

        if (movie.Rating is null || movie.Rating < 1 || movie.Rating > 10)
            throw new ArgumentException("Rating must be between 1 and 10");

        await GetMovieData(movie);

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
    }

    private async Task GetMovieData(Movie movie)
    {
        var key = _config["TMDB:ApiKey"];
        
        if (key == "test-key")
        {
            movie.PosterUrl = "";
            movie.ReleaseYear = "2020";
            movie.Genre = "Test";
            return;
        }

        using var client = new HttpClient();

        var url =
            $"https://api.themoviedb.org/3/search/movie?api_key={key}&query={movie.Title}";

        var json = await client.GetStringAsync(url);

        using JsonDocument doc = JsonDocument.Parse(json);

        var results = doc.RootElement.GetProperty("results");

        if (results.GetArrayLength() == 0)
        {
            movie.PosterUrl = "https://via.placeholder.com/300x450?text=No+Poster";
            movie.ReleaseYear = "-";
            movie.Genre = "Unknown";
            return;
        }

        var first = results[0];

        var posterPath = first.GetProperty("poster_path").GetString();

        movie.PosterUrl =
            string.IsNullOrEmpty(posterPath)
                ? "https://via.placeholder.com/300x450?text=No+Poster"
                : $"https://image.tmdb.org/t/p/w500{posterPath}";

        var releaseDate = first.GetProperty("release_date").GetString();

        movie.ReleaseYear =
            string.IsNullOrWhiteSpace(releaseDate)
                ? "-"
                : releaseDate.Substring(0, 4);

        var genreIds = first.GetProperty("genre_ids");

        movie.Genre = genreIds.GetArrayLength() > 0
            ? GetGenreName(genreIds[0].GetInt32())
            : "Unknown";
    }

    public void DeleteMovie(int id)
    {
        var movie = _context.Movies.FirstOrDefault(x => x.Id == id);

        if (movie == null) return;

        _context.Movies.Remove(movie);
        _context.SaveChanges();
    }
    
    private string GetGenreName(int id)
    {
        return id switch
        {
            28 => "Action",
            12 => "Adventure",
            16 => "Animation",
            35 => "Comedy",
            80 => "Crime",
            18 => "Drama",
            14 => "Fantasy",
            27 => "Horror",
            9648 => "Mystery",
            10749 => "Romance",
            878 => "Sci-Fi",
            53 => "Thriller",
            _ => "Movie"
        };
    }

    public double GetAverageRating()
    {
        if (!_context.Movies.Any()) return 0;

        return _context.Movies.Average(x => x.Rating!.Value);
    }

    public List<Movie> GetAll()
    {
        return _context.Movies.ToList();
    }
    
    public void UpdateRating(int id, int rating)
    {
        if (rating < 1 || rating > 10)
            throw new ArgumentException();

        var movie = _context.Movies.FirstOrDefault(x => x.Id == id);

        if (movie == null) return;

        movie.Rating = rating;

        _context.SaveChanges();
    }
}