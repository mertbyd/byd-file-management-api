using Microsoft.EntityFrameworkCore;
using Byd.FileManagement.Entities.FileCategories;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.FileManagement.EntityFrameworkCore;

[ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
public interface IFileManagementDbContext : IEfCoreDbContext
{
    // Dosya kategorisi aggregate setini repository uygulamasina acar.
    DbSet<FileCategory> FileCategories { get; }
}
