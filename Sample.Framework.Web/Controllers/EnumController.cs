using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Framework.Core.Data;
using Sample.Framework.Core.Models;
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
    public class EnumController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnumController> _logger;

        public EnumController(ApplicationDbContext context, ILogger<EnumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Enum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnumDefinitionDto>>> GetEnums()
        {
            _logger.LogInformation("Attempting to retrieve all enum definitions.");
            try
            {
                var enums = await _context.EnumDefinitions
                    .Include(e => e.Values)
                    .ToListAsync();
                _logger.LogInformation($"Successfully retrieved {enums.Count} enum definitions.");
                return enums.Select(e => e.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enum definitions.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Enum/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnumDefinitionDto>> GetEnum(int id)
        {
            var enumDefinition = await _context.EnumDefinitions
                .Include(e => e.Values)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enumDefinition == null)
            {
                return NotFound();
            }

            return enumDefinition.ToDto();
        }

        // POST: api/Enum
        [HttpPost]
        public async Task<ActionResult<EnumDefinitionDto>> CreateEnum(EnumDefinitionDto enumDefinitionDto)
        {
            var enumDefinition = enumDefinitionDto.ToEntity();
            _context.EnumDefinitions.Add(enumDefinition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnum), new { id = enumDefinition.Id }, enumDefinition.ToDto());
        }

        // PUT: api/Enum/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnum(int id, EnumDefinitionDto enumDefinitionDto)
        {
            if (id != enumDefinitionDto.Id)
            {
                return BadRequest();
            }

            var existingEnum = await _context.EnumDefinitions
                .Include(e => e.Values)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEnum == null)
            {
                return NotFound();
            }

            // 更新枚举基本信息
            existingEnum.Name = enumDefinitionDto.Name;
            existingEnum.Description = enumDefinitionDto.Description;
            existingEnum.Namespace = enumDefinitionDto.Namespace;
            existingEnum.UpdatedAt = DateTime.Now;

            // 删除不再存在的枚举值
            var valuesToRemove = existingEnum.Values
                .Where(v => !enumDefinitionDto.Values.Any(nv => nv.Id == v.Id && nv.Id != 0))
                .ToList();

            foreach (var value in valuesToRemove)
            {
                _context.EnumValues.Remove(value);
            }

            // 更新或添加枚举值
            foreach (var valueDto in enumDefinitionDto.Values)
            {
                var existingValue = existingEnum.Values
                    .FirstOrDefault(v => v.Id == valueDto.Id && valueDto.Id != 0);

                if (existingValue != null)
                {
                    // 更新现有枚举值
                    existingValue.Name = valueDto.Name;
                    existingValue.Value = valueDto.Value;
                    existingValue.Description = valueDto.Description;
                    existingValue.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // 添加新枚举值
                    var newValue = valueDto.ToEntity();
                    newValue.EnumId = id;
                    _context.EnumValues.Add(newValue);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnumExists(id))
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

        // DELETE: api/Enum/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnum(int id)
        {
            var enumDefinition = await _context.EnumDefinitions.FindAsync(id);
            if (enumDefinition == null)
            {
                return NotFound();
            }

            _context.EnumDefinitions.Remove(enumDefinition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnumExists(int id)
        {
            return _context.EnumDefinitions.Any(e => e.Id == id);
        }
    }
}