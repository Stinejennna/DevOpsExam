using System.ComponentModel.DataAnnotations;

namespace DevOpsExamMovie.Models;

public class Movie
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int? Rating { get; set; }
}