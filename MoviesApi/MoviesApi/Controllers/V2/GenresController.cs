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

namespace MoviesApi.Controllers.V2
{
    [Route("api/v2/genres")]
    [ApiController]
    public class GenresV2Controller : CustomBaseController
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenresV2Controller(ILogger<GenresController> logger, ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Get all genres
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "getGenresv2")]
        [ServiceFilter(typeof(LoggingActionFilter))]
        public async Task<ActionResult<List<GenreDto>>> Get()
        {
            return await Get<Genre, GenreDto>();
        }

        /// <summary>
        /// Get a genre by Id
        /// </summary>
        /// <param name="Id">Id of the genre to fetch</param>
        /// <returns></returns>
        [HttpGet("{Id:int}", Name = "getGenrev2")]
        [ProducesResponseType(typeof(GenreDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GenreDto>> Get(int Id)
        {
            return await Get<Genre, GenreDto>(Id);
        }

        /// <summary>
        /// Create a genre
        /// </summary>
        /// <param name="genreCreation">The fields of the genre to create</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromBody] GenreCreationDto genreCreation)
        {
            return await Post<GenreCreationDto, Genre, GenreDto>(genreCreation, "getGenrev2");
        }


        /// <summary>
        /// Update a genre
        /// </summary>
        /// <param name="id">Id of the genre to update</param>
        /// <param name="genreCreation">The fields of the genre to update</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDto genreCreation)
        {
            return await Put<GenreCreationDto, Genre>(id, genreCreation);
        }

        /// <summary>
        /// Delete a genre
        /// </summary>
        /// <param name="id">Id of the genre to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Genre>(id);
        }
    }
}
