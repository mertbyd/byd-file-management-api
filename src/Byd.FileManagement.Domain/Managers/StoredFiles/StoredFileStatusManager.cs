using System;
using Byd.CoreExtensions.Managers;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Models.StoredFiles;

namespace Byd.FileManagement.Managers.StoredFiles;

/// <summary>
/// Dosya durumu lookup'inin normalizasyon ve mutasyon kurallarini yonetir.
/// </summary>
// islevi: Durum adini kanoniklestirir ve entity kurar.
// sistemdeki gorevi: Seed akisini is kararindan ve data-shell entity'yi davranistan uzak tutan domain sahibidir.
public class StoredFileStatusManager : BaseManager<StoredFileStatus, Guid>
{
    // StoredFileStatus extended repository'sini ortak guard tabanina devreder.
    public StoredFileStatusManager(IStoredFileStatusRepository repository)
        : base(repository)
    {
    }

    // Create modelini normalize edip yeni StoredFileStatus entity'sine donusturur.
    // Sistem durumlari sabit kimlikle seed edildigi icin id disaridan verilebilir.
    public virtual StoredFileStatus Create(StoredFileStatusCreateModel model, Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new StoredFileStatus(id ?? GuidGenerator.Create())
        {
            Name = NormalizeRequiredText(model.Name),
            Code = model.Code
        };
    }
}
