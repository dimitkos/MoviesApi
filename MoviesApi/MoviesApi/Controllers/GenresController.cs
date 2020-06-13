using Microsoft.AspNetCore.Mvc;
using MoviesApi.Entities;
using MoviesApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly IRepository _repository;

        public GenresController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            var genres = await _repository.GetAllGenres();

            return Ok(genres);
        }

        [HttpGet("{Id:int}")]
        public ActionResult<Genre> Get(int Id)
        {
            var genre = _repository.GetGenreById(Id);

            if(genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {
            return NoContent();
        }
    }
}
