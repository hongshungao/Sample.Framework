using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Framework.Core.Data;
using Sample.Framework.Core.Models;
using Sample.Framework.Generator;
using Sample.Framework.Web.DTOs;
using Sample.Framework.Web.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Framework.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CodeGenerator _codeGenerator;

        public EntityController(ApplicationDbContext context, CodeGenerator codeGenerator)
        {
            _context = context;
            _codeGenerator = codeGenerator;
        }

        // GET: api/Entity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntityDto>>> GetEntities()
        {
            var entities = await _context.Entities
                .Include(e => e.Properties)
                .ThenInclude(p => p.Enum)
                .ToListAsync();
                
            return entities.Select(e => e.ToDto()).ToList();
        }

        // GET: api/Entity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityDto>> GetEntity(int id)
        {
            var entity = await _context.Entities
                .Include(e => e.Properties)
                .ThenInclude(p => p.Enum)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return entity.ToDto();
        }

        // POST: api/Entity
        [HttpPost]
        public async Task<ActionResult<EntityDto>> CreateEntity(EntityDto entityDto)
        {
            var entity = entityDto.ToEntity();
            _context.Entities.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity.ToDto());
        }

        // PUT: api/Entity/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntity(int id, EntityDto entityDto)
        {
            if (id != entityDto.Id)
            {
                return BadRequest();
            }

            var existingEntity = await _context.Entities
                .Include(e => e.Properties)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            // 更新实体基本信息
            existingEntity.Name = entityDto.Name;
            existingEntity.Description = entityDto.Description;
            existingEntity.Namespace = entityDto.Namespace;
            existingEntity.UpdatedAt = DateTime.Now;

            // 删除不再存在的属性
            var propertiesToRemove = existingEntity.Properties
                .Where(p => !entityDto.Properties.Any(np => np.Id == p.Id && np.Id != 0))
                .ToList();

            foreach (var property in propertiesToRemove)
            {
                _context.EntityProperties.Remove(property);
            }

            // 更新或添加属性
            foreach (var propertyDto in entityDto.Properties)
            {
                var existingProperty = existingEntity.Properties
                    .FirstOrDefault(p => p.Id == propertyDto.Id && propertyDto.Id != 0);

                if (existingProperty != null)
                {
                    // 更新现有属性
                    existingProperty.Name = propertyDto.Name;
                    existingProperty.Type = propertyDto.Type;
                    existingProperty.IsRequired = propertyDto.IsRequired;
                    existingProperty.IsKey = propertyDto.IsKey;
                    existingProperty.Description = propertyDto.Description;
                    existingProperty.EnumId = propertyDto.EnumId;
                    existingProperty.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // 添加新属性
                    var newProperty = propertyDto.ToEntity();
                    newProperty.EntityId = id;
                    _context.EntityProperties.Add(newProperty);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Entity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(int id)
        {
            var entity = await _context.Entities.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _context.Entities.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Entity/Generate
        [HttpPost("Generate")]
        public async Task<ActionResult<IEnumerable<string>>> GenerateCode([FromQuery] int? entityId)
        {
            List<Entity> entities;
            List<EnumDefinition> enums;

            if (entityId.HasValue)
            {
                // 生成单个实体的代码
                var entity = await _context.Entities
                    .Include(e => e.Properties)
                    .ThenInclude(p => p.Enum)
                    .FirstOrDefaultAsync(e => e.Id == entityId);

                if (entity == null)
                {
                    return NotFound();
                }

                entities = new List<Entity> { entity };

                // 获取实体引用的所有枚举
                var enumIds = entity.Properties
                    .Where(p => p.EnumId.HasValue)
                    .Select(p => p.EnumId.Value)
                    .Distinct();

                enums = await _context.EnumDefinitions
                    .Include(e => e.Values)
                    .Where(e => enumIds.Contains(e.Id))
                    .ToListAsync();
            }
            else
            {
                // 生成所有实体和枚举的代码
                entities = await _context.Entities
                    .Include(e => e.Properties)
                    .ThenInclude(p => p.Enum)
                    .ToListAsync();

                enums = await _context.EnumDefinitions
                    .Include(e => e.Values)
                    .ToListAsync();
            }

            // 生成代码
            var generatedFiles = await _codeGenerator.GenerateCodeAsync(entities, enums);

            return Ok(generatedFiles);
        }

        private bool EntityExists(int id)
        {
            return _context.Entities.Any(e => e.Id == id);
        }
    }
}