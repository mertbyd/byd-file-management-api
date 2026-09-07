using System;
using Byd.CoreExtensions.Repositories;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.FileManagement.EntityFrameworkCore.Repositories.FileCategories;

/// <summary>
/// FileCategory extended repository sozlesmesini EF Core DbContext'e baglar.
/// </summary>
// islevi: FileCategory icin generic, bulk ve DataTable repository yeteneklerini saglar.
// sistemdeki gorevi: IFileCategoryRepository sozlesmesinin EfExtendedRepository tabanli provider uygulamasidir.
// mirastan gelen: CRUD, filtreli liste, sayim, kullanim kontrolu ve bulk yazma yuzeyleri.
// burada yazilan: yok - miras yuzeyi FileCategory kullanim senaryolarina yeter.
[ExposeServices(typeof(IFileCategoryRepository))]
public class EfCoreFileCategoryRepository
    : EfExtendedRepository<FileManagementDbContext, FileCategory, Guid>,
        IFileCategoryRepository
{
    // DbContext provider'ini extended repository tabanina devreder.
    public EfCoreFileCategoryRepository(
        IDbContextProvider<FileManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
