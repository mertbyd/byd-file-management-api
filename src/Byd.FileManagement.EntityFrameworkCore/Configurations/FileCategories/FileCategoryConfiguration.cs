using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.FileCategories;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Byd.FileManagement.EntityFrameworkCore.Configurations.FileCategories;

/// <summary>
/// FileCategory entity'sinin EF Core tablo ve kolon sozlesmesini tanimlar.
/// </summary>
// islevi: Dosya kategorisi tablo adini, semasini ve kolon kisitlarini map eder.
// sistemdeki gorevi: FileCategory tablosunun PostgreSQL semasindaki tek EF konfigurasyon sahibidir.
public sealed class FileCategoryConfiguration : IEntityTypeConfiguration<FileCategory>
{
    // ABP convention alanlarindan sonra FileCategory kolon sozlesmesini uygular.
    public void Configure(EntityTypeBuilder<FileCategory> builder)
    {
        builder.ToTable(FileCategoryConsts.TableName, FileManagementDbProperties.DbSchema);
        builder.ConfigureByConvention();
        builder.Property(entity => entity.Name)
            .IsRequired()
            .HasMaxLength(FileCategoryConsts.MaxNameLength);
    }
}
