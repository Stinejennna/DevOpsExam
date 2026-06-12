using System.ComponentModel.DataAnnotations;

namespace DevOpsExamMovie.Models;

public class CreateMovieRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int? Rating { get; set; }

    public string? PosterUrl { get; set; }

    public string? ReleaseYear { get; set; }

    public string? Genre { get; set; }
}