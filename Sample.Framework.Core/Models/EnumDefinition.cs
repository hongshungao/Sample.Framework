using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sample.Framework.Core.Models
{
    /// <summary>
    /// 表示一个枚举类型的定义
    /// </summary>
    /// <summary>
    /// 枚举定义，用于存储用户定义的枚举信息
    /// </summary>
    public class EnumDefinition : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(100)]
        public string? Namespace { get; set; }
        
        public virtual ICollection<EnumValue> Values { get; set; } = new List<EnumValue>();
    }
}