using MoviesApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public interface IRepository
    {
        Task<List<Genre>> GetAllGenres();
        Genre GetGenreById(int Id);
    }
}
