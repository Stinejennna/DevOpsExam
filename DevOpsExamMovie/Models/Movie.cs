namespace DevOpsExamMovie.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Rating { get; set; } // 1–10
}