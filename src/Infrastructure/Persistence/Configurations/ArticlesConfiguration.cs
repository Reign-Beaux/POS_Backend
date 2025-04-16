using Domain.Entities.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ArticlesConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");

            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.Name).HasMaxLength(64);
            builder.Property(c => c.Description).HasMaxLength(256);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.IsDeleted).HasDefaultValue(false);

            builder.Property(c => c.ArticleTypeId).IsRequired();
            builder.Property(c => c.BrandId).IsRequired();
            builder.Property(c => c.Stock).HasPrecision(18, 5).HasDefaultValue(0m);
            builder.Property(c => c.MinStockLevel).HasPrecision(18, 5).HasDefaultValue(0m);
            builder.Property(c => c.MaxStockLevel).HasPrecision(18, 5).HasDefaultValue(0m);
            builder.Property(c => c.Barcode).HasMaxLength(64).HasDefaultValue(string.Empty);

            builder.HasOne(c => c.ArticleType)
                .WithMany(at => at.Articles)
                .HasForeignKey(c => c.ArticleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Brand)
                .WithMany(at => at.Articles)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
