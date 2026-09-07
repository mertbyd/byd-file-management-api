using Microsoft.EntityFrameworkCore;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Entities.StoredFiles;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.FileManagement.EntityFrameworkCore;

[ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
public interface IFileManagementDbContext : IEfCoreDbContext
{
    // Dosya kategorisi aggregate setini repository uygulamasina acar.
    DbSet<FileCategory> FileCategories { get; }

    // Saklanan dosya metadata aggregate setini repository uygulamasina acar.
    DbSet<StoredFile> StoredFiles { get; }

    // Dosya durumu lookup setini repository uygulamasina acar.
    DbSet<StoredFileStatus> StoredFileStatuses { get; }
}
