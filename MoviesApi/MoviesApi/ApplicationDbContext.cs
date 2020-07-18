using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.PersonId, x.MovieId });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
    }
}
