using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.StoredFiles;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Byd.FileManagement.EntityFrameworkCore.Configurations.StoredFiles;

/// <summary>
/// StoredFile entity'sinin EF Core tablo, kolon ve indeks sozlesmesini tanimlar.
/// </summary>
// islevi: Dosya metadata tablo adini, semasini, alan sinirlarini, kategori iliskisini ve okuma
// indekslerini map eder.
// sistemdeki gorevi: StoredFile tablosunun PostgreSQL semasindaki tek EF konfigurasyon sahibidir.
public sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    // ABP convention alanlarindan sonra StoredFile kolon, iliski ve indeks sozlesmesini uygular.
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable(StoredFileConsts.TableName, FileManagementDbProperties.DbSchema);
        builder.ConfigureByConvention();
        builder.Property(entity => entity.ModuleName)
            .HasMaxLength(StoredFileConsts.MaxModuleNameLength);
        builder.Property(entity => entity.BlobName)
            .HasMaxLength(StoredFileConsts.MaxBlobNameLength);
        builder.Property(entity => entity.OriginalFileName)
            .HasMaxLength(StoredFileConsts.MaxOriginalFileNameLength);

        // Kategori silinirse ona bagli dosya metadatasi sessizce kaybolmaz; silme engellenir.
        builder.HasOne(entity => entity.Category)
            .WithMany()
            .HasForeignKey(entity => entity.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Bir is kaydinin ayni kategoride birden fazla dosyasi olabilir: kategori bir belge
        // turudur, tek dosyalik bir yuva degil. Indeks okuma icindir, teklik zorlamaz.
        // ExternalId bas kolon oldugu icin yalniz is kaydiyla yapilan sorgular da bunu kullanir.
        builder.HasIndex(entity => new { entity.ExternalId, entity.CategoryId })
            .HasDatabaseName(StoredFileConsts.ExternalCategoryIndex);

        // Entity IMultiTenant oldugu icin ABP her sorguya tenant kosulunu ekler; indeks onu karsilar.
        builder.HasIndex(entity => entity.TenantId)
            .HasDatabaseName(StoredFileConsts.TenantIndex);
    }
}
