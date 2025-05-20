using Microsoft.EntityFrameworkCore;
using Sample.Framework.Core.Models;

namespace Sample.Framework.Core.Data
{
    /// <summary>
    /// 应用程序数据库上下文，用于管理实体和枚举的数据访问
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Entity> Entities { get; set; } = null!;
        public DbSet<EntityProperty> EntityProperties { get; set; } = null!;
        public DbSet<EnumDefinition> EnumDefinitions { get; set; } = null!;
        public DbSet<EnumValue> EnumValues { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置实体关系
            modelBuilder.Entity<Entity>()
                .HasMany(e => e.Properties)
                .WithOne(p => p.Entity)
                .HasForeignKey(p => p.EntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EnumDefinition>()
                .HasMany(e => e.Values)
                .WithOne(v => v.Enum)
                .HasForeignKey(v => v.EnumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EntityProperty>()
                .HasOne(p => p.Enum)
                .WithMany()
                .HasForeignKey(p => p.EnumId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}