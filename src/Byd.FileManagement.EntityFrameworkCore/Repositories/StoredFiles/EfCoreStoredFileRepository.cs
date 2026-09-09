using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Byd.CoreExtensions.Repositories;
using Byd.FileManagement.Consts;
using Byd.CoreExtensions.Querying;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.FileManagement.EntityFrameworkCore.Repositories.StoredFiles;

/// <summary>
/// IStoredFileRepository sozlesmesini EF Core DbContext'e baglar.
/// </summary>
// islevi: StoredFile icin generic, bulk, DataTable ve kategori kirilimli okuma sorgularini
// saglar; okuma yollarini onaylanmis kayitlarla sinirlar.
// sistemdeki gorevi: IStoredFileRepository sozlesmesinin EfExtendedRepository tabanli provider uygulamasidir.
// mirastan gelen: CRUD, filtreli liste, sayim, kullanim kontrolu ve bulk yazma yuzeyleri.
// burada yazilan: is kaydi kimlikleriyle toplu okuma, kategori kirilimli okuma ve onay kisiti.
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

    // StoredFile okuma sorgularina DTO'nun CategoryName ve StatusName kaynaklarini ekler.
    public override async Task<IQueryable<StoredFile>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(entity => entity.Category)
            .Include(entity => entity.Status);
    }

    // Is kaydi kimliklerinin onaylanmis dosyalarini veritabaninda tek sorguyla getirir.
    public virtual async Task<List<StoredFile>> GetListByExternalIdsAsync(
        IEnumerable<Guid> externalIds,
        CancellationToken cancellationToken = default)
    {
        return await ByExternalIds(OnlyApproved(await WithDetailsAsync()), externalIds)
            .ToListAsync(cancellationToken);
    }

    // Is kaydinin verilen kategorideki onaylanmis dosyalarini getirir.
    public virtual async Task<List<StoredFile>> GetListByCategoryAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await InCategory(OnlyApproved(await WithDetailsAsync()), externalId, categoryId)
            .ToListAsync(cancellationToken);
    }

    // Verilen kategorideki onaylanmis dosyalari veritabaninda filtreleyerek getirir.
    public virtual async Task<List<StoredFile>> GetListByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await OnlyApproved(await WithDetailsAsync())
            .Where(entity => entity.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    // Filtreli listede kategori adini ayni sorguda getirir ve onaysiz kayitlari disarida
    // birakir. Taban kancasi hem listeyi hem sayimi besledigi icin kural tek yerde durur.
    protected override async Task<IQueryable<StoredFile>> CreateFilteredQueryAsync<TFiltering>(
        IDataTablePagingProperty<TFiltering> pagingProperty)
    {
        return OnlyApproved(await base.CreateFilteredQueryAsync(pagingProperty))
            .Include(entity => entity.Category);
    }

    // Byte'lari henuz yazilmamis kayitlar okuma yollarinda gorunmez; yarim kalan yukleme
    // var olmayan bir blob'a isaret eder.
    private static IQueryable<StoredFile> OnlyApproved(IQueryable<StoredFile> query)
    {
        return query.Where(entity => entity.StatusId == StoredFileStatusConsts.ApprovedId);
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
