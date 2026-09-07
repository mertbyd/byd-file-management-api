using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.StoredFiles;
using Byd.FileManagement.Models.StoredFiles;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Byd.FileManagement.Data.StoredFiles;

/// <summary>
/// Dosya durumu lookup'inin iki sistem satirini idempotent olarak olusturur.
/// </summary>
// islevi: Beklemede ve Onayli satirlarindan eksik olanlari sabit kimliklerle kurup toplu ekler.
// sistemdeki gorevi: Yukleme akisi bu iki kimlige dogrudan yazdigi icin satirlarin varligi zorunludur;
// eksiklerse dosya yuklenemez.
public class StoredFileStatusDataSeeder : IDataSeedContributor, ITransientDependency
{
    private static readonly IReadOnlyList<StatusSeed> Seeds =
    [
        new(StoredFileStatusConsts.PendingId, StoredFileStatusConsts.PendingSeedName, StoredFileStatusConsts.PendingCode),
        new(StoredFileStatusConsts.ApprovedId, StoredFileStatusConsts.ApprovedSeedName, StoredFileStatusConsts.ApprovedCode)
    ];

    private readonly StoredFileStatusManager _manager;
    private readonly IStoredFileStatusRepository _repository;

    public StoredFileStatusDataSeeder(
        StoredFileStatusManager manager,
        IStoredFileStatusRepository repository)
    {
        _manager = manager;
        _repository = repository;
    }

    // Eksik durum satirlarini tek okuma ve tek bulk yazma ile ekler.
    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var existing = await _repository.GetListAsync(
            includeDetails: false,
            cancellationToken: CancellationToken.None);
        var existingIds = new HashSet<Guid>();
        foreach (var status in existing)
        {
            existingIds.Add(status.Id);
        }

        var missing = CreateMissingStatuses(existingIds);
        if (missing.Count == 0)
        {
            return;
        }

        await _repository.BulkInsertManyAsync(missing, batchSize: null, CancellationToken.None);
    }

    // Mevcut kimlikleri koruyarak yalnizca eksik seed entity'lerini kurar.
    private List<StoredFileStatus> CreateMissingStatuses(ISet<Guid> existingIds)
    {
        var missing = new List<StoredFileStatus>();
        foreach (var seed in Seeds)
        {
            if (existingIds.Contains(seed.Id))
            {
                continue;
            }

            missing.Add(_manager.Create(
                new StoredFileStatusCreateModel { Name = seed.Name, Code = seed.Code },
                seed.Id));
        }

        return missing;
    }

    // Tek bir sistem durumunun sabit seed tanimi.
    private sealed record StatusSeed(Guid Id, string Name, int Code);
}
