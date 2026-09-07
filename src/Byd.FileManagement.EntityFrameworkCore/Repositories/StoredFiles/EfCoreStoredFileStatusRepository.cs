using System;
using Byd.CoreExtensions.Repositories;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.FileManagement.EntityFrameworkCore.Repositories.StoredFiles;

/// <summary>
/// IStoredFileStatusRepository sozlesmesini EF Core DbContext'e baglar.
/// </summary>
// islevi: StoredFileStatus icin generic, bulk ve DataTable repository yeteneklerini saglar.
// sistemdeki gorevi: IStoredFileStatusRepository sozlesmesinin EfExtendedRepository tabanli uygulamasidir.
// mirastan gelen: CRUD, filtreli liste, sayim, kullanim kontrolu ve bulk yazma yuzeyleri.
// burada yazilan: yok - miras yuzeyi durum lookup'ina yeter.
[ExposeServices(typeof(IStoredFileStatusRepository))]
public class EfCoreStoredFileStatusRepository
    : EfExtendedRepository<FileManagementDbContext, StoredFileStatus, Guid>,
        IStoredFileStatusRepository
{
    // DbContext provider'ini extended repository tabanina devreder.
    public EfCoreStoredFileStatusRepository(
        IDbContextProvider<FileManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
