using Microsoft.EntityFrameworkCore;
using Byd.CoreExtensions.EntityFrameworkCore;
using Byd.CoreExtensions.Extensions;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Entities.StoredFiles;
using Volo.Abp.Data;

namespace Byd.FileManagement.EntityFrameworkCore;

[ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
public class FileManagementDbContext : BydDbContext<FileManagementDbContext>, IFileManagementDbContext
{
    // FileCategory repository'sinin calistigi aggregate setidir.
    public DbSet<FileCategory> FileCategories { get; set; }

    // StoredFile repository'sinin calistigi aggregate setidir.
    public DbSet<StoredFile> StoredFiles { get; set; }

    // StoredFileStatus repository'sinin calistigi lookup setidir.
    public DbSet<StoredFileStatus> StoredFileStatuses { get; set; }

    public FileManagementDbContext(DbContextOptions<FileManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureFileManagement();
        builder.ApplySnakeCaseNaming();
    }
}
