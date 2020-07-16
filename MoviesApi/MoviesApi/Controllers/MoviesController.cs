using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Services;
using System.Collections.Generic;
using System.IO;
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
        private readonly string containerName = "movies";

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

        [HttpGet("{id}", Name = "getMovie")]
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

        [HttpPost]
        public async Task<ActionResult> Post([FromForm]MovieCreationDto movieCreation)
        {
            var movie = _mapper.Map<Movie>(movieCreation);

            if (movieCreation.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreation.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreation.Poster.FileName);
                    movie.Poster =
                        await _fileStorageService.SaveFile(content, extension, containerName,
                                                            movieCreation.Poster.ContentType);
                }
            }

            _context.Add(movie);

            await _context.SaveChangesAsync();

            var movieDto = _mapper.Map<MovieDto>(movie);

            return new CreatedAtRouteResult("getMovie", new { Id = movie.Id }, movieDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDto movieCreation)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            movie = _mapper.Map(movieCreation, movie);

            if (movieCreation.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreation.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreation.Poster.FileName);
                    movie.Poster =
                        await _fileStorageService.SaveFile(content, extension, containerName,
                                                            movieCreation.Poster.ContentType);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDto> jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDb = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDb == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<MoviePatchDto>(entityFromDb);

            //apply our changes to entity dto
            jsonPatchDocument.ApplyTo(entityDTO, ModelState);

            //validating all the business rules
            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(entityDTO, entityFromDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Movies.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Movie() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
