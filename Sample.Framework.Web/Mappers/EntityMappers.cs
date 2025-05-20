using Sample.Framework.Core.Models;
using Sample.Framework.Web.DTOs;
using System;
using System.Linq;

namespace Sample.Framework.Web.Mappers
{
    /// <summary>
    /// 实体模型与DTO之间的映射工具类
    /// </summary>
    public static class EntityMappers
    {
        /// <summary>
        /// 将实体模型转换为DTO
        /// </summary>
        public static EntityDto ToDto(this Entity entity)
        {
            return new EntityDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Namespace = entity.Namespace,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Properties = entity.Properties?.Select(p => p.ToDto()).ToList() ?? new List<EntityPropertyDto>()
            };
        }

        /// <summary>
        /// 将DTO转换为实体模型
        /// </summary>
        public static Entity ToEntity(this EntityDto dto)
        {
            return new Entity
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Namespace = dto.Namespace,
                CreatedAt = dto.Id == 0 ? DateTime.Now : dto.CreatedAt,
                UpdatedAt = dto.Id == 0 ? null : DateTime.Now,
                Properties = dto.Properties?.Select(p => p.ToEntity()).ToList() ?? new List<EntityProperty>()
            };
        }

        /// <summary>
        /// 将实体属性模型转换为DTO
        /// </summary>
        public static EntityPropertyDto ToDto(this EntityProperty property)
        {
            return new EntityPropertyDto
            {
                Id = property.Id,
                Name = property.Name,
                Type = property.Type,
                IsRequired = property.IsRequired,
                IsKey = property.IsKey,
                Description = property.Description,
                EnumId = property.EnumId,
                Enum = property.Enum?.ToDto(),
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt
            };
        }

        /// <summary>
        /// 将DTO转换为实体属性模型
        /// </summary>
        public static EntityProperty ToEntity(this EntityPropertyDto dto)
        {
            return new EntityProperty
            {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.Type,
                IsRequired = dto.IsRequired,
                IsKey = dto.IsKey,
                Description = dto.Description,
                EnumId = dto.EnumId,
                CreatedAt = dto.Id == 0 ? DateTime.Now : dto.CreatedAt,
                UpdatedAt = dto.Id == 0 ? null : DateTime.Now
            };
        }

        /// <summary>
        /// 将枚举定义模型转换为DTO
        /// </summary>
        public static EnumDefinitionDto ToDto(this EnumDefinition enumDefinition)
        {
            return new EnumDefinitionDto
            {
                Id = enumDefinition.Id,
                Name = enumDefinition.Name,
                Description = enumDefinition.Description,
                Namespace = enumDefinition.Namespace,
                CreatedAt = enumDefinition.CreatedAt,
                UpdatedAt = enumDefinition.UpdatedAt,
                Values = enumDefinition.Values?.Select(v => v.ToDto()).ToList() ?? new List<EnumValueDto>()
            };
        }

        /// <summary>
        /// 将DTO转换为枚举定义模型
        /// </summary>
        public static EnumDefinition ToEntity(this EnumDefinitionDto dto)
        {
            return new EnumDefinition
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Namespace = dto.Namespace,
                CreatedAt = dto.Id == 0 ? DateTime.Now : dto.CreatedAt,
                UpdatedAt = dto.Id == 0 ? null : DateTime.Now,
                Values = dto.Values?.Select(v => v.ToEntity()).ToList() ?? new List<EnumValue>()
            };
        }

        /// <summary>
        /// 将枚举值模型转换为DTO
        /// </summary>
        public static EnumValueDto ToDto(this EnumValue enumValue)
        {
            return new EnumValueDto
            {
                Id = enumValue.Id,
                Name = enumValue.Name,
                Value = enumValue.Value,
                Description = enumValue.Description,
                CreatedAt = enumValue.CreatedAt,
                UpdatedAt = enumValue.UpdatedAt
            };
        }

        /// <summary>
        /// 将DTO转换为枚举值模型
        /// </summary>
        public static EnumValue ToEntity(this EnumValueDto dto)
        {
            return new EnumValue
            {
                Id = dto.Id,
                Name = dto.Name,
                Value = dto.Value,
                Description = dto.Description,
                CreatedAt = dto.Id == 0 ? DateTime.Now : dto.CreatedAt,
                UpdatedAt = dto.Id == 0 ? null : DateTime.Now
            };
        }
    }
}