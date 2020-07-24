using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApi.DTOs;
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
        private readonly IMapper _mapper;

        public GenresController(ILogger<GenresController> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(LoggingActionFilter))]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[ResponseCache(Duration = 60)]
        public async Task<ActionResult<List<GenreDto>>> Get()
        {
            var genres = await _context.Genres.AsNoTracking().ToListAsync();

            var genresDto = _mapper.Map<List<GenreDto>>(genres);

            return Ok(genresDto);
        }

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDto>> Get(int Id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == Id);

            if (genre == null)
            {
                return NotFound();
            }

            var genresDto = _mapper.Map<GenreDto>(genre);

            return Ok(genresDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDto genreCreation)
        {
            var genre = _mapper.Map<Genre>(genreCreation);

            _context.Genres.Add(genre);
            //_context.Add(genre);
            await _context.SaveChangesAsync();

            var genresDto = _mapper.Map<GenreDto>(genre);

            return new CreatedAtRouteResult("getGenre", new { Id = genresDto.Id }, genresDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDto genreCreation)
        {
            var genre = _mapper.Map<Genre>(genreCreation);

            genre.Id = id;

            _context.Entry(genre).State = EntityState.Modified;
            genre.Name = genreCreation.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Genres.AnyAsync(x => x.Id == id);

            if(!exists)
            {
                return NotFound();
            }

            _context.Genres.Remove(new Genre { Id = id});

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
