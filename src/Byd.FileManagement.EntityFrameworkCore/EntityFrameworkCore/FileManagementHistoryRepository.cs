using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Byd.FileManagement.EntityFrameworkCore;

/// <summary>
/// Modul migration gecmisini cercevenin varsayilan gecmis tablosuna yazar.
/// </summary>
// islevi: Gecmis tablosunun kolon adlarini snake_case donusumunden muaf tutar; MigrationId ve ProductVersion adlarini sabitler.
// sistemdeki gorevi: Tuketen host ve core ile ayni veritabanini paylasirken tek ortak migration gecmisi tablosunun kullanilmasini saglar.
public class FileManagementHistoryRepository : NpgsqlHistoryRepository
{
    public FileManagementHistoryRepository(HistoryRepositoryDependencies dependencies)
        : base(dependencies)
    {
    }

    // Gecmis tablosunun kolon adlarini cerceve varsayilanina sabitler.
    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);
        history.Property(row => row.MigrationId).HasColumnName(DefaultMigrationIdColumnName);
        history.Property(row => row.ProductVersion).HasColumnName(DefaultProductVersionColumnName);
    }

    private const string DefaultMigrationIdColumnName = "MigrationId";
    private const string DefaultProductVersionColumnName = "ProductVersion";
}
