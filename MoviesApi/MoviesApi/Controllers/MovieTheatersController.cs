using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/movietheaters")]
    public class MovieTheatersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public MovieTheatersController(IMapper mapper, ApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _context = applicationDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDto>>> Get([FromQuery] FilterMovieTheatersDto filterMovieTheatersDTO)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var usersLocation = geometryFactory
                .CreatePoint(new Coordinate(filterMovieTheatersDTO.Long, filterMovieTheatersDTO.Lat));

            var theaters = await _context.MovieTheaters
                .OrderBy(x => x.Location.Distance(usersLocation))
                .Where(x => x.Location.IsWithinDistance(usersLocation, filterMovieTheatersDTO.DistanceInKms * 1000))
                .Select(x => new MovieTheaterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DistanceInMeters = Math.Round(x.Location.Distance(usersLocation))
                })
                .ToListAsync();

            return theaters;
        }
    }
}
