using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class MovieDetailsDto :MovieDto
    {
        public List<GenreDto> Genres { get; set; }
        public List<ActorDto> Actors { get; set; }
    }
}
