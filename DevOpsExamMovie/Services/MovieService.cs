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
        ValidateMovie(movie);
        await GetMovieData(movie);

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
    }

    private static void ValidateMovie(Movie movie)
    {
        if (string.IsNullOrWhiteSpace(movie.Title))
            throw new ArgumentException("Title is required");

        if (movie.Rating is null)
            throw new ArgumentException("Rating is required");

        ValidateRating(movie.Rating.Value);
    }

    private async Task GetMovieData(Movie movie)
    {
        var key = _config["TMDB:ApiKey"];
        
        if (key == "test-key")
        {
            SetTestMovieData(movie);
            return;
        }

        using var client = new HttpClient();
        var url = $"https://api.themoviedb.org/3/search/movie?api_key={key}&query={movie.Title}";
        var json = await client.GetStringAsync(url);

        using JsonDocument doc = JsonDocument.Parse(json);
        var results = doc.RootElement.GetProperty("results");

        if (results.GetArrayLength() == 0)
        {
            SetNoResultsMovieData(movie);
            return;
        }

        ExtractMovieDataFromResult(movie, results[0]);
    }

    private static void SetTestMovieData(Movie movie)
    {
        movie.PosterUrl = "";
        movie.ReleaseYear = "2020";
        movie.Genre = "Test";
    }

    private static void SetNoResultsMovieData(Movie movie)
    {
        const string placeholderUrl = "https://via.placeholder.com/300x450?text=No+Poster";
        movie.PosterUrl = placeholderUrl;
        movie.ReleaseYear = "-";
        movie.Genre = "Unknown";
    }

    private static void ExtractMovieDataFromResult(Movie movie, JsonElement result)
    {
        var posterPath = result.GetProperty("poster_path").GetString();
        const string placeholderUrl = "https://via.placeholder.com/300x450?text=No+Poster";

        movie.PosterUrl = string.IsNullOrEmpty(posterPath)
            ? placeholderUrl
            : $"https://image.tmdb.org/t/p/w500{posterPath}";

        var releaseDate = result.GetProperty("release_date").GetString();
        movie.ReleaseYear = string.IsNullOrWhiteSpace(releaseDate)
            ? "-"
            : releaseDate[..4];

        var genreIds = result.GetProperty("genre_ids");
        movie.Genre = genreIds.GetArrayLength() > 0
            ? GetGenreName(genreIds[0].GetInt32())
            : "Unknown";
    }

    public async Task DeleteMovie(int id)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null) return;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
    }
    
    private static string GetGenreName(int id)
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

    public async Task<double> GetAverageRating()
    {
        var hasMovies = await _context.Movies.AnyAsync();
        if (!hasMovies) return 0;

        return await _context.Movies.AverageAsync(x => x.Rating!.Value);
    }

    public async Task<List<Movie>> GetAll()
    {
        return await _context.Movies.ToListAsync();
    }
    
    public async Task UpdateRating(int id, int rating)
    {
        ValidateRating(rating);

        var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null) return;

        movie.Rating = rating;
        await _context.SaveChangesAsync();
    }

    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 10)
            throw new ArgumentException("Rating must be between 1 and 10.", nameof(rating));
    }
}