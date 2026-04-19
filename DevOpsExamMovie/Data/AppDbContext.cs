using DevOpsExamMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsExamMovie.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();
}