using System.Collections.Generic;

namespace MoviesApi.DTOs
{
    public class IndexMoviePageDto
    {
        public List<MovieDto> UpcomingRealeases { get; set; }
        public List<MovieDto> InTheaters { get; set; }
    }
}
