using MoviesApi.DTOs;
using System.Linq;

namespace MoviesApi.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queyable, PaginationDto pagination)
        {
            return queyable
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage);
        }
    }
}
