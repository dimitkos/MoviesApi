using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public async Task<ActionResult<IndexMoviePageDto>> Get()
        {
            var top = 6;
            var today = DateTime.Today;

            var upcomingRealeases = await _context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var inTheaters = await _context.Movies
                .Where(x => x.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new IndexMoviePageDto
            {
                UpcomingRealeases = _mapper.Map<List<MovieDto>>(upcomingRealeases),
                InTheaters = _mapper.Map<List<MovieDto>>(inTheaters)
            };

            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDto>>> Filter([FromQuery] FilterMoviesDto filterMoviesDto)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDto.Title))
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDto.Title));

            if (filterMoviesDto.InTheaters)
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);

            if (filterMoviesDto.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMoviesDto.GenreId != 0)
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(filterMoviesDto.GenreId));

            //use pagination logic after the filter logic
            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMoviesDto.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDto.Pagination).ToListAsync();

            var moviesDto = _mapper.Map<List<MovieDto>>(movies);

            return Ok(moviesDto);
        }

        [HttpGet("{id}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDto>> Get(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(movieDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDto movieCreation)
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

            AnnotateActorsOrder(movie);

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

            await _context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movie.Id}; delete from MoviesGenres where MovieId = {movie.Id}");

            AnnotateActorsOrder(movie);

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

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }
    }
}
