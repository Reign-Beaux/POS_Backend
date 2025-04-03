using Domain.Entities.ArticleTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ArticleTypesConfiguration
    {
        public void Configure(EntityTypeBuilder<ArticleType> builder)
        {
            builder.ToTable("ArticleTypes");
                
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.Name).HasMaxLength(64);
            builder.Property(c => c.Description).HasMaxLength(256);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
