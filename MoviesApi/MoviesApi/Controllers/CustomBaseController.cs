using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected async Task<ActionResult<List<TResponse>>> Get<TEntity, TResponse>() where TEntity : class
        {
            var entities = await _context.Set<TEntity>()
                                    .AsNoTracking()
                                    .ToListAsync();

            var dtos = _mapper.Map<List<TResponse>>(entities);

            return Ok(dtos);
        }

        protected async Task<ActionResult<List<TResponse>>> Get<TEntity, TResponse>(PaginationDto pagination) where TEntity : class
        {
            var queryable = _context.Set<TEntity>().AsNoTracking().AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);
            var entities = await queryable.Paginate(pagination).ToListAsync();
            var dtos = _mapper.Map<List<TResponse>>(entities);

            return Ok(dtos);
        }

        protected async Task<ActionResult<TResponse>> Get<TEntity, TResponse>(int Id) where TEntity : class, IId
        {
            var entity = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);

            if (entity == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<TResponse>(entity);

            return Ok(dto);
        }

        protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(TCreation creation, string routeName) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(creation);

            _context.Add(entity);
            await _context.SaveChangesAsync();

            var readDto = _mapper.Map<TRead>(entity);

            return new CreatedAtRouteResult(routeName, new { Id = entity.Id }, readDto);
        }

        protected async Task<ActionResult> Put<TCreation, TEntity>(int id, TCreation creation) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(creation);

            entity.Id = id;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
        {
            var exists = await _context.Set<TEntity>().AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new TEntity() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity, TDTO>(int id, JsonPatchDocument<TDTO> jsonPatchDocument) where TDTO : class
            where TEntity : class, IId
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDb = await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDb == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<TDTO>(entityFromDb);

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
    }
}
