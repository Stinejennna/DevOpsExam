using System.ComponentModel.DataAnnotations;

namespace DevOpsExamMovie.Models;

public class RatingUpdate
{
    [Required]
    [Range(1, 10)]
    public int? Rating { get; set; }
}