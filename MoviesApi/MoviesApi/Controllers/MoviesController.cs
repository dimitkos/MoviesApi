using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieDto>>> Get()
        {
            var movies = await _context.Movies.AsNoTracking().ToListAsync();

            var moviesDto = _mapper.Map<List<MovieDto>>(movies);

            return Ok(moviesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> Get(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if(movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<MovieDto>(movie);

            return Ok(movieDto);
        }
    }
}
