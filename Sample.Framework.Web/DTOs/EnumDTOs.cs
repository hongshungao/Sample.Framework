using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sample.Framework.Web.DTOs
{
    /// <summary>
    /// 枚举定义DTO，用于API的数据传输
    /// </summary>
    public class EnumDefinitionDto
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
        
        public List<EnumValueDto> Values { get; set; } = new List<EnumValueDto>();
    }

    /// <summary>
    /// 枚举值DTO，用于API的数据传输
    /// </summary>
    public class EnumValueDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int Value { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}