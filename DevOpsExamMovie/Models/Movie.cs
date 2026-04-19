using System.ComponentModel.DataAnnotations;

namespace DevOpsExamMovie.Models;

public class Movie
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1,10)]
    public int? Rating { get; set; }
}