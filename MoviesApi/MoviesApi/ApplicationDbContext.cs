using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<Movie> Movies { get; set; }
    }
}
