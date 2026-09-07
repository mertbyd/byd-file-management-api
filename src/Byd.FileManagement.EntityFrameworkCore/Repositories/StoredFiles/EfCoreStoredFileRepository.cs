using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Byd.CoreExtensions.Repositories;
using Byd.CoreExtensions.Querying;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.FileManagement.EntityFrameworkCore.Repositories.StoredFiles;

/// <summary>
/// IStoredFileRepository sozlesmesini EF Core DbContext'e baglar.
/// </summary>
// islevi: StoredFile icin generic, bulk, DataTable ve slot sorgularini saglar.
// sistemdeki gorevi: IStoredFileRepository sozlesmesinin EfExtendedRepository tabanli provider uygulamasidir.
// mirastan gelen: CRUD, filtreli liste, sayim, kullanim kontrolu ve bulk yazma yuzeyleri.
// burada yazilan: is kaydi kimlikleriyle toplu okuma ve tekil slot sorgusu.
[ExposeServices(typeof(IStoredFileRepository))]
public class EfCoreStoredFileRepository
    : EfExtendedRepository<FileManagementDbContext, StoredFile, Guid>,
        IStoredFileRepository
{
    // DbContext provider'ini extended repository tabanina devreder.
    public EfCoreStoredFileRepository(
        IDbContextProvider<FileManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // StoredFile okuma sorgularina DTO'nun CategoryName kaynagini ekler.
    public override async Task<IQueryable<StoredFile>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(entity => entity.Category);
    }

    // Is kaydi kimliklerinin dosyalarini veritabaninda tek sorguyla getirir.
    public virtual async Task<List<StoredFile>> GetListByExternalIdsAsync(
        IEnumerable<Guid> externalIds,
        CancellationToken cancellationToken = default)
    {
        return await ByExternalIds(await WithDetailsAsync(), externalIds)
            .ToListAsync(cancellationToken);
    }

    // Slotu isgal eden kaydi getirir.
    public virtual async Task<StoredFile?> GetBySlotAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await InSlot(await WithDetailsAsync(), externalId, categoryId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    // Filtreli listede kategori adini ayni sorguda getirir.
    protected override async Task<IQueryable<StoredFile>> CreateFilteredQueryAsync<TFiltering>(
        IDataTablePagingProperty<TFiltering> pagingProperty)
    {
        return (await base.CreateFilteredQueryAsync(pagingProperty))
            .Include(entity => entity.Category);
    }

    private static IQueryable<StoredFile> ByExternalIds(
        IQueryable<StoredFile> query,
        IEnumerable<Guid> externalIds)
    {
        var ids = externalIds.ToArray();

        return query.Where(entity => ids.Contains(entity.ExternalId));
    }

    private static IQueryable<StoredFile> InSlot(
        IQueryable<StoredFile> query,
        Guid externalId,
        Guid categoryId)
    {
        return query.Where(entity => entity.ExternalId == externalId && entity.CategoryId == categoryId);
    }
}
