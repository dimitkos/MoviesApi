using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "people";

        public PeopleController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDto>>> Get([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);

            var people = await queryable.Paginate(pagination).ToListAsync();

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
        public async Task<ActionResult> Post([FromForm] PersonCreationDto personCreation)
        {
            var person = _mapper.Map<Person>(personCreation);

            if (personCreation.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreation.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreation.Picture.FileName);
                    person.Picture =
                        await _fileStorageService.SaveFile(content, extension, containerName,
                                                            personCreation.Picture.ContentType);
                }
            }

            _context.People.Add(person);
            //_context.Add(person);
            await _context.SaveChangesAsync();

            var personDto = _mapper.Map<PersonDto>(person);

            return new CreatedAtRouteResult("getPerson", new { Id = personDto.Id }, personDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PersonCreationDto personCreation)
        {
            var personDb = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (personDb == null)
            {
                return NotFound();
            }

            var personDB = _mapper.Map(personCreation, personDb);

            if (personCreation.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreation.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreation.Picture.FileName);
                    personDB.Picture =
                        await _fileStorageService.EditFile(content, extension, containerName,
                                                            personDB.Picture,
                                                            personCreation.Picture.ContentType);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDto> jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDb = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDb == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<PersonPatchDto>(entityFromDb);

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
            var exists = await _context.People.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Person() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
