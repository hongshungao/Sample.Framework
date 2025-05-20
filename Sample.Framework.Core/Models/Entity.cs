using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Framework.Core.Models
{
    /// <summary>
    /// 实体基类，所有实体都继承自此类
    /// </summary>
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// 实体定义，用于存储用户定义的实体信息
    /// </summary>
    public class Entity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(100)]
        public string? Namespace { get; set; }
        
        public virtual ICollection<EntityProperty> Properties { get; set; } = new List<EntityProperty>();
    }

    /// <summary>
    /// 实体属性，用于存储实体的属性信息
    /// </summary>
    public class EntityProperty : BaseEntity
    {
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
        
        public int EntityId { get; set; }
        
        [ForeignKey("EntityId")]
        public virtual Entity Entity { get; set; } = null!;
        
        public int? EnumId { get; set; }
        
        [ForeignKey("EnumId")]
        public virtual EnumDefinition? Enum { get; set; }
    }

   

}