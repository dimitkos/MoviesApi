using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApi.Entities;
using MoviesApi.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ApplicationDbContext _context;

        public GenresController(ILogger<GenresController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ServiceFilter(typeof(LoggingActionFilter))]
        //[ResponseCache(Duration = 60)]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            var genres = await _context.Genres.AsNoTracking().ToListAsync();

            return Ok(genres);
        }

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<Genre>> Get(int Id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == Id);

            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Genre genre)
        {
            _context.Genres.Add(genre);
            //_context.Add(genre);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("getGenre", new { Id = genre.Id }, genre);
        }
    }
}
