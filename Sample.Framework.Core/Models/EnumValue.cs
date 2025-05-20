using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Framework.Core.Models
{
    /// <summary>
    /// 表示枚举类型中的一个具体值
    /// </summary>
   
    public class EnumValue : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int Value { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int EnumId { get; set; }
        
        [ForeignKey("EnumId")]
        public virtual EnumDefinition Enum { get; set; } 
    }
}