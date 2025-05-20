using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sample.Framework.Web.DTOs
{
    /// <summary>
    /// 实体DTO，用于API的数据传输
    /// </summary>
    public class EntityDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(100)]
        public string? Namespace { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public List<EntityPropertyDto> Properties { get; set; } = new List<EntityPropertyDto>();
    }

    /// <summary>
    /// 实体属性DTO，用于API的数据传输
    /// </summary>
    public class EntityPropertyDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;
        
        public bool IsRequired { get; set; }
        
        public bool IsKey { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int? EnumId { get; set; }
        
        public EnumDefinitionDto? Enum { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}