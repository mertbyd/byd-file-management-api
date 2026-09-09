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
// islevi: StoredFile icin generic, bulk, DataTable ve kategori kirilimli okuma sorgularini saglar.
// sistemdeki gorevi: IStoredFileRepository sozlesmesinin EfExtendedRepository tabanli provider uygulamasidir.
// mirastan gelen: CRUD, filtreli liste, sayim, kullanim kontrolu ve bulk yazma yuzeyleri.
// burada yazilan: is kaydi kimlikleriyle toplu okuma ve kategori kirilimli okuma.
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
        return (await GetQueryableAsync())
            .Include(entity => entity.Category);
    }

    // Is kaydi kimliklerinin dosyalarini veritabaninda tek sorguyla getirir.
    public virtual async Task<List<StoredFile>> GetListByExternalIdsAsync(
        IEnumerable<Guid> externalIds,
        CancellationToken cancellationToken = default)
    {
        return await ByExternalIds(await WithDetailsAsync(), externalIds)
            .ToListAsync(cancellationToken);
    }

    // Is kaydinin verilen kategorideki dosyalarini getirir.
    public virtual async Task<List<StoredFile>> GetListByCategoryAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await InCategory(await WithDetailsAsync(), externalId, categoryId)
            .ToListAsync(cancellationToken);
    }

    // Verilen kategorideki dosyalari veritabaninda filtreleyerek getirir.
    public virtual async Task<List<StoredFile>> GetListByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await (await WithDetailsAsync())
            .Where(entity => entity.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    // Filtreli listede kategori adini ayni sorguda getirir. Taban kancasi hem listeyi hem
    // sayimi besledigi icin kural tek yerde durur.
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

    private static IQueryable<StoredFile> InCategory(
        IQueryable<StoredFile> query,
        Guid externalId,
        Guid categoryId)
    {
        return query.Where(entity => entity.ExternalId == externalId && entity.CategoryId == categoryId);
    }
}
