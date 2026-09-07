using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Entities.StoredFiles;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Byd.FileManagement.EntityFrameworkCore.Configurations.StoredFiles;

/// <summary>
/// StoredFile entity'sinin EF Core tablo, kolon ve indeks sozlesmesini tanimlar.
/// </summary>
// islevi: Dosya metadata tablo adini, semasini, alan sinirlarini, kategori iliskisini ve slot
// indeksini map eder.
// sistemdeki gorevi: StoredFile tablosunun PostgreSQL semasindaki tek EF konfigurasyon sahibidir.
public sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    // ABP convention alanlarindan sonra StoredFile kolon, iliski ve indeks sozlesmesini uygular.
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable(StoredFileConsts.TableName, FileManagementDbProperties.DbSchema);
        builder.ConfigureByConvention();
        builder.Property(entity => entity.ModuleName)
            .IsRequired()
            .HasMaxLength(StoredFileConsts.MaxModuleNameLength);
        builder.Property(entity => entity.BlobName)
            .IsRequired()
            .HasMaxLength(StoredFileConsts.MaxBlobNameLength);
        builder.Property(entity => entity.OriginalFileName)
            .IsRequired()
            .HasMaxLength(StoredFileConsts.MaxOriginalFileNameLength);

        // Kategori silinirse ona bagli dosya metadatasi sessizce kaybolmaz; silme engellenir.
        builder.HasOne<FileCategory>()
            .WithMany()
            .HasForeignKey(entity => entity.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Bir is kaydinin bir slotunda tek dosya bulunur; kural veritabaninda zorlanir.
        builder.HasIndex(entity => new { entity.ExternalId, entity.CategoryId })
            .IsUnique()
            .HasDatabaseName(StoredFileConsts.SlotIndex);
        builder.HasIndex(entity => entity.TenantId)
            .HasDatabaseName(StoredFileConsts.TenantIndex);
    }
}
