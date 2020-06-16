using MoviesApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public interface IRepository
    {
        void AddGenre(Genre genre);
        Task<List<Genre>> GetAllGenres();
        Genre GetGenreById(int Id);
    }
}
