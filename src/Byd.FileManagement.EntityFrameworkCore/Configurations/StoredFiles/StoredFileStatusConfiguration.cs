using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.StoredFiles;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Byd.FileManagement.EntityFrameworkCore.Configurations.StoredFiles;

/// <summary>
/// StoredFileStatus lookup'inin EF Core tablo ve kolon sozlesmesini tanimlar.
/// </summary>
// islevi: Durum tablo adini, semasini ve kolon kisitlarini map eder.
// sistemdeki gorevi: StoredFileStatus tablosunun PostgreSQL semasindaki tek EF konfigurasyon sahibidir.
public sealed class StoredFileStatusConfiguration : IEntityTypeConfiguration<StoredFileStatus>
{
    // ABP convention alanlarindan sonra StoredFileStatus kolon sozlesmesini uygular.
    public void Configure(EntityTypeBuilder<StoredFileStatus> builder)
    {
        builder.ToTable(StoredFileStatusConsts.TableName, FileManagementDbProperties.DbSchema);
        builder.ConfigureByConvention();
        builder.Property(entity => entity.Name)
            .IsRequired()
            .HasMaxLength(StoredFileStatusConsts.MaxNameLength);
    }
}
