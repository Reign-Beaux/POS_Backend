using Application.Interfaces.Context;
using Domain.Entities.KeysForTranslations;
using Domain.Entities.Languages;
using Domain.Entities.Translations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public partial class PosUtilsDbContext(DbContextOptions<PosUtilsDbContext> options) : DbContext(options), IPosUtilsDbContext
{
    public virtual DbSet<KeysForTranslation> KeysForTranslations { get; set; }
    public virtual DbSet<Language> Languages { get; set; }
    public virtual DbSet<Translation> Translations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeysForTranslation>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_KeysForTranslations_Name");

            entity.HasIndex(e => e.Name, "UQ__KeysForT__737584F605465E1B").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Languages_Name");

            entity.HasIndex(e => e.Name, "UQ__Language__737584F626A7355B").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.HasIndex(e => new { e.KeysForTranslationId, e.LanguageId }, "IX_Translations_Key_Lang");

            entity.HasIndex(e => new { e.KeysForTranslationId, e.LanguageId }, "UQ_Translations_Key_Lang").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Text)
                .HasMaxLength(4096)
                .IsUnicode(false);

            entity.HasOne(d => d.KeysForTranslation).WithMany(p => p.Translations)
                .HasForeignKey(d => d.KeysForTranslationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Translations_KeysForTranslations");

            entity.HasOne(d => d.Language).WithMany(p => p.Translations)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Translations_Languages");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}
