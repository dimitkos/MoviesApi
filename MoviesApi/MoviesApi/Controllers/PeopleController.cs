using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PeopleController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDto>>> Get()
        {
            var people = await _context.People.AsNoTracking().ToListAsync();

            var peopleDto = _mapper.Map<List<PersonDto>>(people);

            return Ok(peopleDto);
        }

        [HttpGet("{id:int}", Name = "getPerson")]
        public async Task<ActionResult<PersonDto>> Get(int Id)
        {
            var person = await _context.Genres.FirstOrDefaultAsync(x => x.Id == Id);

            if (person == null)
            {
                return NotFound();
            }

            var personDto = _mapper.Map<PersonDto>(person);

            return Ok(personDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PersonCreationDto personCreation)
        {
            var person = _mapper.Map<Person>(personCreation);

            _context.People.Add(person);
            //_context.Add(genre);
            await _context.SaveChangesAsync();

            var personDto = _mapper.Map<PersonDto>(person);

            return new CreatedAtRouteResult("getPerson", new { Id = personDto.Id }, personDto);
        }
    }
}
